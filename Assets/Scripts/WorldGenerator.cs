using CielaSpike;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
[Serializable]
public struct NoiseParameters
{
    //public string name;
    public  int seed;
    public  float amplitude;
    public  float frequency;
    public float persistence;
    public  float lacunarity;
    public  int octaves;
    public void setSeed(int sd)
    {
        seed = sd;
    }
}
public enum tileNames : int
{
    dirt = 0,
    grass,
    sand,
    deep_water,
    shallow_water,
    tilled_dirt
};
public class WorldGenerator : MonoBehaviour
{
    [SerializeField]
    float structuresOdds;
    [SerializeField]
    List<Tilemap> tilemaps;
    /*
    Tilemap groundTilemap;
    [SerializeField]
    Tilemap waterTilemap;*/
    [SerializeField]
    TileBase[] tiles;
    //Noise noise;
    [SerializeField]
    int seed;
    [SerializeField]
    bool randomizeSeed;
    [SerializeField]
    int defaultChunkSize;
    [SerializeField]
    Vector2Int qInit;
    Vector2Int prevQInit;
    [SerializeField]
    int cullDistance;
    //List of flattened chunks, containing all data to be displayed on screen
    List<List<tileNames>> chunkTiles;
    //List of chunk centers
    HashSet<chunkCenter> loadedChunks;
    CameraFollow camFollow;
    float3 playerPos;
    [SerializeField]
    List<NoiseParameters> noiseParameterLists;
    int moistureNoiseIndex;
    int heightNoiseIndex;
    [SerializeField]
    List<Biome> biomes;
    /*
    [SerializeField]
    NoiseParameters heightmapNoise;
    [SerializeField]
    NoiseParameters biomeNoise;*/
    /*
    [SerializeField]
    List<Tilemap> structureTilemaps;*/
    /*[SerializeField]
    List<Structure> structures;*/
    [SerializeField]
    List<chunkCenter> chunksToGenerate;
    [SerializeField]
    ChunkPersistenceHandler cpHandler;
    System.Random random;
    private StructureSaver saver;

    [Serializable]
    readonly struct chunkCenter
    {
        public readonly int boundsCenterX;
        public readonly int boundsCenterY;
        public chunkCenter(int bX, int bY)
        {
            boundsCenterX = bX;
            boundsCenterY = bY;
        }
    }
    private void Awake()
    {
        saver = GetComponent<StructureSaver>();
        cpHandler = GetComponent<ChunkPersistenceHandler>();
    }
    // Start is called before the first frame update
    void Start()
    {
        camFollow = CameraFollow.instance;
        loadedChunks = new HashSet<chunkCenter>();
        loadedChunks.Add(new chunkCenter(qInit.x,qInit.y));
        chunkCenter origin = new chunkCenter(qInit.x, qInit.y);
        random = new System.Random();
        moistureNoiseIndex = 1;
        heightNoiseIndex = 0;
        int index = 0;
        foreach (var nosP in noiseParameterLists)
        {
            if (randomizeSeed) nosP.setSeed((int)UnityEngine.Random.Range(-123456789f, 123456789f));
            /*if (nosP.name == "Moisture") moistureNoiseIndex = index;
            else if (nosP.name == "Height") heightNoiseIndex = index;*/
            index++;
        }   

        GenerateMap(new List<chunkCenter> { origin },defaultChunkSize);
        StartCoroutine(chunkManager());
        this.StartCoroutineAsync(chunkCuller());
    }

    private IEnumerator chunkCuller()
    {
        while(true)
        {
            Vector2Int playerPosInt = new Vector2Int((int)camFollow.playerPos.x,(int)camFollow.playerPos.y);
            float dist =  Vector2Int.Distance(qInit, prevQInit);
            if (dist > defaultChunkSize * cullDistance)
            {
                yield return Ninja.JumpToUnity;
                float stagger = 0f;
                loadedChunks.RemoveWhere((chunkCenter chunk) => {

                    if(Vector2Int.Distance(playerPosInt, new Vector2Int(chunk.boundsCenterX, chunk.boundsCenterY)) > defaultChunkSize * cullDistance)
                    {
                        stagger += 2f;
                        StartCoroutine(cullChunk(chunk,stagger));
                        return true;
                    }
                    return false;
                });
                yield return Ninja.JumpBack;
                prevQInit = qInit;
            }
            yield return new WaitForSeconds(2f);
        }
    }
    private bool chunkExistsOnFile(chunkCenter c)
    {
        DecompressedChunk chunkParams = cpHandler.LoadChunkFromDisk(c.boundsCenterX / defaultChunkSize, c.boundsCenterY / defaultChunkSize, defaultChunkSize);
        if (chunkParams == null) return false;
        SetTilesForChunk(chunkParams.tilemapIndices, chunkParams.tileLists, chunkParams.positionLists);
        return true;
        //Chunk.ChunkExists(c.boundsCenterX/defaultChunkSize, c.boundsCenterY/defaultChunkSize);
    }
    private JobHandle GenerateNewChunk(chunkCenter c, int size, List<NativeArray<float>> noiseLists,NoiseParameters nosP)
    {
        NativeArray<float> NoiseValues = new NativeArray<float>(size * size, Allocator.TempJob);
        GenerateMapJob job = new GenerateMapJob
        (
            c.boundsCenterX,
            c.boundsCenterY,
            size,
            nosP,
            NoiseValues
        );
        noiseLists.Add(NoiseValues);
        return job.Schedule();
    }
    //Pass in a list of chunks to be generated, and it will call jobs to get noise values for 
    private void GenerateMap(List<chunkCenter> cCenters,int size)
    {

        if (cCenters.Count <= 0) return;
        NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);
        List<List<NativeArray<float>>> tileNoiseLists = new List<List<NativeArray<float>>>();
        int index = 0;
        bool newTilesGenerated = false;
        foreach(var noise in noiseParameterLists)
        {
            tileNoiseLists.Add(new List<NativeArray<float>>());
        }
        foreach (var c in cCenters)
        {
            if (!chunkExistsOnFile(c))
            {
                index = 0;
                foreach(var nList in tileNoiseLists)
                {
                    jobHandleList.Add(GenerateNewChunk(c, size, nList,noiseParameterLists[index++]));
                }
                newTilesGenerated = true;
            }
            
        }

        JobHandle.CompleteAll(jobHandleList);
        if (newTilesGenerated)
        {
            StartCoroutine(SetMapTiles(tileNoiseLists, cCenters, size));
        }
    } 
    private Biome getBiomeForLocation(float moistureNoise)
    {
        foreach(var biome in biomes)
        {
            if(moistureNoise > biome.minMoisture && moistureNoise <= biome.maxMoisture)
            {
                return biome;
            }
        }
        return biomes[0];
    }
    private IEnumerator SetMapTiles(List<List<NativeArray<float>>> tileNoiseLists, List<chunkCenter> cCenters, int size)
    {
        yield return null;

        List<Vector3Int[]> positionArrays = new List<Vector3Int[]>(tilemaps.Count);
        for(int i = 0; i < tilemaps.Count; i++)
        {
            positionArrays.Add(new Vector3Int[size * size]);
        }
        List<TileBase[]> tileArrays = new List<TileBase[]>(tilemaps.Count);
        for(int i = 0; i < tilemaps.Count; i++)
        {
            tileArrays.Add(new TileBase[size * size]);
        }
        int index, noiseListNum = 0;
        int boundsCenterX, boundsCenterY;
        Biome curBiome;
        tilemapData curTileData;
        HashSet<int> modifiedTilemaps;
        int randIndex;
        foreach (var NoiseValues in tileNoiseLists[heightNoiseIndex])
        {
            randIndex = random.Next(tileNoiseLists[moistureNoiseIndex][noiseListNum].Count());
            modifiedTilemaps = new HashSet<int>();
            //GenerateStructures(cCenters[noiseListNum],NoiseValues.ToList());
            boundsCenterY = cCenters[noiseListNum].boundsCenterY;
            boundsCenterX = cCenters[noiseListNum].boundsCenterX;
            index = 0;
            curBiome = getBiomeForLocation(tileNoiseLists[moistureNoiseIndex][noiseListNum][randIndex]);
            foreach (var curMapData in curBiome.tilemaps)
            {
                if (!modifiedTilemaps.Contains(curMapData.tilemapIndex))
                {
                    modifiedTilemaps.Add(curMapData.tilemapIndex);
                }
                for (int x = boundsCenterX - size / 2; x < boundsCenterX + size / 2; x++)
                {
                    for (int y = boundsCenterY - size / 2; y < boundsCenterY + size / 2; y++)
                    {
                        positionArrays[curMapData.tilemapIndex][index] = new Vector3Int(x, y, 0);
                        tileArrays[curMapData.tilemapIndex][index] = curMapData.getTileForHeight(NoiseValues[index]);
                        index++;
                    }
                }
            }
            SetTilesForChunk(modifiedTilemaps, tileArrays, positionArrays);
            cpHandler.CreateAndSaveChunk(boundsCenterX/defaultChunkSize, boundsCenterY/defaultChunkSize, tileArrays,modifiedTilemaps);
            //Debug.Log(positionArrays);
            /*Chunk c = new Chunk(boundsCenterX/defaultChunkSize, boundsCenterY/defaultChunkSize, tileArrays,positionArrays);
            c.SaveToFile();*/
            NoiseValues.Dispose();
            tileNoiseLists[moistureNoiseIndex][noiseListNum++].Dispose();
        }
        //groundTilemap.SetTile(new Vector3Int(0, 0, 0), tiles[(int)tileNames.grass]);
        //SaveChunks(cCenters);
        /*waterTilemap.gameObject.SetActive(false);
        waterTilemap.gameObject.SetActive(true);*/
    }
    private void SetTilesForChunk(HashSet<int> modifiedTilemaps,List<TileBase[]> tileArrays,List<Vector3Int[]> positionArrays)
    {
        foreach (var ind2 in modifiedTilemaps)
        {
            tilemaps[ind2].SetTiles(positionArrays[ind2], tileArrays[ind2]);
        }
    }
    private void GenerateStructures(chunkCenter cCenter, List<float> noiseValues)
    {
        /*
        int randDex = random.Next(noiseValues.Count);
        if(noiseValues[randDex] > structuresOdds)
        {
            int index = random.Next(structures.Count);
            //structures[index]
            int ind = 0;
            List<List<Vector3Int>> positionLists = new List<List<Vector3Int>>();
            foreach(var positionList in structures[index].positionLists)
            {
                positionLists.Add(new List<Vector3Int>());
                foreach(Vector3Int vector in positionList.list)
                {
                    positionLists[ind].Add(vector + new Vector3Int(cCenter.boundsCenterX, cCenter.boundsCenterY,0));
                }
                ind++;
            }
            int ind2 = 0;
            foreach (var tilemap in structureTilemaps)
            {
                tilemap.SetTiles(positionLists[ind2].ToArray(),structures[index].tileLists[ind2].list.ToArray());
                ind2++;
            }
        }*/
    }
    private void checkForChunks(List<chunkCenter> chunksToGenerate, Vector3 playerPos,chunkCenter qNE, chunkCenter qE, chunkCenter qW, chunkCenter qSE, chunkCenter qS, chunkCenter qN, chunkCenter qNW, chunkCenter qSW)
    {

        //top right corner
        if (!loadedChunks.Contains(qNE))
        {
            //qNE
            chunksToGenerate.Add(qNE);
            loadedChunks.Add(qNE);
        }
        if (!loadedChunks.Contains(qE))
        {
            //qE
            chunksToGenerate.Add(qE);
            loadedChunks.Add(qE);
        }
        if (!loadedChunks.Contains(qW))
        {
            //qW
            chunksToGenerate.Add(qW);
            loadedChunks.Add(qW);
        }
        if (!loadedChunks.Contains(qSE))
        {
            //qSE
            chunksToGenerate.Add(qSE);
            loadedChunks.Add(qSE);
        }
        if (!loadedChunks.Contains(qN))
        {
            //qN
            chunksToGenerate.Add(qN);
            loadedChunks.Add(qN);
        }
        if (!loadedChunks.Contains(qNW))
        {
            //qNW
            chunksToGenerate.Add(qNW);
            loadedChunks.Add(qNW);
        }
        if (!loadedChunks.Contains(qS))
        {
            //qS
            chunksToGenerate.Add(qS);
            loadedChunks.Add(qS);
        }
        if (!loadedChunks.Contains(qSW))
        {
            //qSW
            chunksToGenerate.Add(qSW);
            loadedChunks.Add(qSW);
        }
    }
    private void changeCenterChunk(Vector3 playerPos, chunkCenter qNE, chunkCenter qE, chunkCenter qW, chunkCenter qSE, chunkCenter qS, chunkCenter qN, chunkCenter qNW, chunkCenter qSW)
    {
        //top right corner
        if (playerPos.x > qInit.x + defaultChunkSize / 2 && playerPos.y > qInit.y + defaultChunkSize / 2)
        {
            //qNE
            qInit = new Vector2Int(qNE.boundsCenterX, qNE.boundsCenterY);
        }
        else if (playerPos.x > qInit.x + defaultChunkSize / 2 && playerPos.y < qInit.y - defaultChunkSize / 2)
        {
            //qSE
            qInit = new Vector2Int(qSE.boundsCenterX, qSE.boundsCenterY);
        }
        else if (playerPos.y > qInit.y + defaultChunkSize / 2 && playerPos.x < qInit.x - defaultChunkSize / 2)
        {
            //qNW
            qInit = new Vector2Int(qNW.boundsCenterX, qNW.boundsCenterY);
        }
        else if (playerPos.x < qInit.x - defaultChunkSize / 2 && playerPos.y < qInit.y - defaultChunkSize / 2)
        {
            //qSW
            qInit = new Vector2Int(qSW.boundsCenterX, qSW.boundsCenterY);
        }
        else if (playerPos.x > qInit.x + defaultChunkSize / 2)
        {
            //qE
            qInit = new Vector2Int(qE.boundsCenterX, qE.boundsCenterY);
        }
        else if (playerPos.x < qInit.x - defaultChunkSize / 2)
        {
            //qW
            qInit = new Vector2Int(qW.boundsCenterX, qW.boundsCenterY);
        }

        else if ( playerPos.y > qInit.y + defaultChunkSize / 2)
        {
            //qN
            qInit = new Vector2Int(qN.boundsCenterX, qN.boundsCenterY);
        }
        
        else if (playerPos.y < qInit.y - defaultChunkSize / 2)
        {
            //qS
            qInit = new Vector2Int(qS.boundsCenterX, qS.boundsCenterY);
        }
       
    }
    private IEnumerator cullChunk(chunkCenter cCenter,float stagger)
    {
        yield return stagger;
        Debug.Log("culling chunk: (" + cCenter.boundsCenterX.ToString() + "," + cCenter.boundsCenterY.ToString()+")");

        int boundsCenterX = cCenter.boundsCenterX;
        int boundsCenterY = cCenter.boundsCenterY;
        int size = defaultChunkSize;
        List<Vector3Int> positionList = new List<Vector3Int>();
        List<TileBase> nullList = new List<TileBase>();
        for (int x = boundsCenterX - size / 2; x < boundsCenterX + size / 2; x++)
        {
            for (int y = boundsCenterY - size / 2; y < boundsCenterY + size / 2; y++)
            {
                positionList.Add(new Vector3Int(x, y, 0));
                nullList.Add(null);
            }
        }
        foreach(var tilemap in tilemaps)
        {
            tilemap.SetTiles(positionList.ToArray(), nullList.ToArray());
        }


    }
    private void chunkManagerHelper(Vector2Int lastGenerationPosition,chunkCenter qNE, chunkCenter qE, chunkCenter qW, chunkCenter qSE, chunkCenter qS, chunkCenter qN, chunkCenter qNW, chunkCenter qSW)
    {
        checkForChunks(chunksToGenerate, playerPos, qNE, qE, qW, qSE, qS, qN, qNW, qSW);
        GenerateMap(chunksToGenerate, defaultChunkSize);
        lastGenerationPosition = new Vector2Int((int)playerPos.x, (int)playerPos.y);
    }
    // Update is called once per frame
    private IEnumerator chunkManager()
    {
        Vector2Int lastGenerationPosition = new Vector2Int((int)camFollow.playerPos.x,(int)camFollow.playerPos.y);
        chunkCenter qNE = new chunkCenter(qInit.x + defaultChunkSize, qInit.y + defaultChunkSize);
        chunkCenter qE = new chunkCenter(qInit.x + defaultChunkSize, qInit.y);
        chunkCenter qW = new chunkCenter(qInit.x - defaultChunkSize, qInit.y);
        chunkCenter qSE = new chunkCenter(qInit.x + defaultChunkSize, qInit.y - defaultChunkSize);
        chunkCenter qS = new chunkCenter(qInit.x, qInit.y - defaultChunkSize);
        chunkCenter qN = new chunkCenter(qInit.x, qInit.y + defaultChunkSize);
        chunkCenter qNW = new chunkCenter(qInit.x - defaultChunkSize, qInit.y + defaultChunkSize);
        chunkCenter qSW = new chunkCenter(qInit.x - defaultChunkSize, qInit.y - defaultChunkSize);
        chunkManagerHelper(lastGenerationPosition, qNE, qE, qW, qSE, qS, qN, qNW, qSW);
        while (true)
        {
            playerPos = camFollow.playerPos;
            //List<chunkCenter> chunksToGenerate = new List<chunkCenter>();
            chunksToGenerate = new List<chunkCenter>();
            qNE = new chunkCenter(qInit.x + defaultChunkSize, qInit.y + defaultChunkSize);
            qE = new chunkCenter(qInit.x + defaultChunkSize, qInit.y);
            qW = new chunkCenter(qInit.x - defaultChunkSize, qInit.y);
            qSE = new chunkCenter(qInit.x + defaultChunkSize, qInit.y - defaultChunkSize);
            qS = new chunkCenter(qInit.x, qInit.y - defaultChunkSize);
            qN = new chunkCenter(qInit.x, qInit.y + defaultChunkSize);
            qNW = new chunkCenter(qInit.x - defaultChunkSize, qInit.y + defaultChunkSize);
            qSW = new chunkCenter(qInit.x - defaultChunkSize, qInit.y - defaultChunkSize);

            if (Vector2Int.Distance(qInit,new Vector2Int((int)playerPos.x,(int)playerPos.y)) > defaultChunkSize/2)
            {
                changeCenterChunk(playerPos,qNE, qE, qW, qSE, qS, qN, qNW, qSW);
            }
            if (Vector2Int.Distance(lastGenerationPosition, new Vector2Int((int)playerPos.x, (int)playerPos.y)) > defaultChunkSize / 4)
            {
                chunkManagerHelper(lastGenerationPosition, qNE, qE, qW, qSE, qS, qN, qNW, qSW);
            }
            yield return new WaitForSeconds(1f);
        }
    }
}

public struct GenerateMapJob : IJob
{
    private int bX;
    private int bY;
    private int sz;
    /*

    private int sd;
    private float amp;
    private float freq;
    private float pers;
    private float lac;
    private int oct;*/
    public NoiseParameters noise;
    public NativeArray<float> noiseValueList;
    /*
    public NoiseParameters moistureNoise;
    public NativeArray<float> moistureNoiseValues;*/
    public GenerateMapJob(int boundsCenterX, int boundsCenterY, int size, NoiseParameters nos, NativeArray<float> nvLists)
    {

        bX = boundsCenterX;
        bY = boundsCenterY;
        sz = size;
        /*
        sd = nos.seed;
        amp = nos.amplitude;
        freq = nos.frequency;
        pers = nos.persistence;
        lac = nos.lacunarity;
        oct = nos.octaves;*/
        noise = nos;
        noiseValueList = nvLists;
    }
    public void Execute()
    {
        GenerateNoise(noise, noiseValueList);
    }
    public void GenerateNoise(NoiseParameters p, NativeArray<float> nosValues)
    {
        float amp = p.amplitude;
        float freq = p.frequency;
        int oct = p.octaves;
        float lac = p.lacunarity;
        float pers = p.persistence;
        int sd = p.seed;
        float max = 0f;
        float min = float.MaxValue;
        int index = 0;
        for (int i = bX - sz / 2; i < bX + sz / 2; i++)
        {
            for (int j = bY - sz / 2; j < bY + sz / 2; j++)
            {
                float tempA = amp;
                float tempF = freq;
                nosValues[index] = 0f;
                for (int k = 0; k < oct; k++)
                {
                    nosValues[index] += Mathf.PerlinNoise(((i + sd) / ((float)sz * freq)), (j + sd) / ((float)sz * freq)) * amp;
                    freq *= lac;
                    amp *= pers;
                }
                amp = tempA;
                freq = tempF;
                if (nosValues[index] > max) max = nosValues[index];
                if (nosValues[index] < min) min = nosValues[index];
                index++;
            }
        }
        index = 0;
        for (int i = 0; i < sz; i++)
        {
            for (int j = 0; j < sz; j++)
            {
                nosValues[index] = Mathf.InverseLerp(max, min, nosValues[index]);
            }
        }
    }
}