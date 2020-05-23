using CielaSpike;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public readonly struct NoiseParameters
{
    public readonly int sd;
    public readonly float amp;
    public readonly float freq;
    public readonly float pers;
    public readonly float lac;
    public readonly int oct;
    public NoiseParameters(int seed, float amplitude, float frequency, float persistence, float lacunarity, int octaves)
    {
        sd = seed;
        amp = amplitude;
        freq = frequency;
        pers = persistence;
        lac = lacunarity;
        oct = octaves;
    }
}
public class WorldGenerator : MonoBehaviour
{
    [SerializeField]
    float structuresOdds;
    [SerializeField]
    Tilemap groundTilemap;
    [SerializeField]
    Tilemap waterTilemap;
    [SerializeField]
    TileBase[] tiles;
    Noise noise;
    [SerializeField]
    int seed;
    [SerializeField]
    float frequency;
    [SerializeField]
    float amplitude;
    [SerializeField]
    float lacunarity;
    [SerializeField]
    float persistence;
    [SerializeField]
    int octaves;
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
    NoiseParameters nosP;
    [SerializeField]
    List<Tilemap> structureTilemaps;
    [SerializeField]
    List<Structure> structures;
    [SerializeField]
    List<chunkCenter> chunksToGenerate;
    System.Random random;
    enum tileNames : int
    {
        dirt = 0,
        grass,
        sand,
        deep_water,
        shallow_water,
        tilled_dirt
    };
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

    // Start is called before the first frame update
    void Start()
    {
        camFollow = CameraFollow.instance;
        loadedChunks = new HashSet<chunkCenter>();
        loadedChunks.Add(new chunkCenter(qInit.x,qInit.y));
        //noise = new Noise(seed, frequency, amplitude, lacunarity, persistence, octaves);
        chunkCenter origin = new chunkCenter(qInit.x, qInit.y);
        //seed = (int)UnityEngine.Random.Range(-123456789f, 123456789f);
        nosP = new NoiseParameters(seed, frequency, amplitude, lacunarity, persistence, octaves);

        random = new System.Random();
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
        return false;
    }
    private JobHandle GenerateNewChunk(chunkCenter c, int size, List<NativeArray<float>> noiseLists)
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
        List<NativeArray<float>> noiseLists = new List<NativeArray<float>>();
        foreach (var c in cCenters)
        {
            if (!chunkExistsOnFile(c))
            {
                jobHandleList.Add(GenerateNewChunk(c, size, noiseLists));
            }
            
        }

        JobHandle.CompleteAll(jobHandleList);
        StartCoroutine(SetMapTiles(noiseLists, cCenters, size));
    } 
    private IEnumerator SetMapTiles(List<NativeArray<float>> noiseLists, List<chunkCenter> cCenters, int size)
    {
        yield return null;
        Vector3Int[] groundPositionArray = new Vector3Int[size * size];
        TileBase[] groundTileArray = new TileBase[size * size];
        Vector3Int[] waterPositionArray = new Vector3Int[size * size];
        TileBase[] waterTileArray = new TileBase[size * size];
        int index, noiseListNum = 0;
        int boundsCenterX, boundsCenterY;
        foreach (var NoiseValues in noiseLists)
        {
            GenerateStructures(cCenters[noiseListNum],NoiseValues.ToList());
            boundsCenterY = cCenters[noiseListNum].boundsCenterY;
            boundsCenterX = cCenters[noiseListNum++].boundsCenterX;
            index = 0;
            for (int x = boundsCenterX - size / 2; x < boundsCenterX + size / 2; x++)
            {
                for (int y = boundsCenterY - size / 2; y < boundsCenterY + size / 2; y++)
                {
                    if (NoiseValues[index] < 0.5f)
                    {
                        groundTileArray[index] = tiles[(int)tileNames.grass];
                        groundPositionArray[index] = new Vector3Int(x, y, 0);
                    }
                    else
                    {
                        waterTileArray[index] = tiles[(int)tileNames.shallow_water];
                        waterPositionArray[index] = new Vector3Int(x, y, 0);
                    }
                    index += 1;

                }
            }
            groundTilemap.SetTiles(groundPositionArray, groundTileArray);
            waterTilemap.SetTiles(waterPositionArray, waterTileArray);
            NoiseValues.Dispose();

        }
        groundTilemap.SetTile(new Vector3Int(0, 0, 0), tiles[(int)tileNames.grass]);
        /*waterTilemap.gameObject.SetActive(false);
        waterTilemap.gameObject.SetActive(true);*/
    }
    private void GenerateStructures(chunkCenter cCenter, List<float> noiseValues)
    {
        int randDex = random.Next(noiseValues.Count);
        if(noiseValues[randDex] > structuresOdds)
        {
            int index = random.Next(structures.Count);
            //structures[index]
            int ind = 0;
            List<Vector3List> positionLists = new List<Vector3List>();
            foreach(var positionList in structures[index].positionLists)
            {
                positionLists.Add(new Vector3List(new List<Vector3Int>()));
                foreach(var vector in positionList.list)
                {
                    positionLists[ind].list.Add(vector + new Vector3Int(cCenter.boundsCenterX, cCenter.boundsCenterY, 0));
                }
                ind++;
            }
            int ind2 = 0;
            foreach (var tilemap in structureTilemaps)
            {
                //structures[index].tileLists
                tilemap.SetTiles(positionLists[ind2].list.ToArray(),structures[index].tileLists[ind2].list.ToArray());
                ind2++;
            }
        }
    }
    private void checkForChunks(List<chunkCenter> chunksToGenerate, Vector3 playerPos,chunkCenter qNE, chunkCenter qE, chunkCenter qW, chunkCenter qSE, chunkCenter qS, chunkCenter qN, chunkCenter qNW, chunkCenter qSW)
    {

        //top right corner
        if (!loadedChunks.Contains(qNE))// && playerPos.x > qInit.x + defaultChunkSize / 4 && playerPos.y > qInit.y + defaultChunkSize / 4)
        {
            //qNE
            chunksToGenerate.Add(qNE);
            loadedChunks.Add(qNE);
        }
        if (!loadedChunks.Contains(qE))// && playerPos.x > qInit.x + defaultChunkSize / 4)
        {
            //qE
            chunksToGenerate.Add(qE);
            loadedChunks.Add(qE);
        }
        if (!loadedChunks.Contains(qW))// && playerPos.x < qInit.x - defaultChunkSize / 4)
        {
            //qW
            chunksToGenerate.Add(qW);
            loadedChunks.Add(qW);
        }
        if (!loadedChunks.Contains(qSE))// && playerPos.x > qInit.x + defaultChunkSize / 4 && playerPos.y < qInit.y - defaultChunkSize / 4)
        {
            //qSE
            chunksToGenerate.Add(qSE);
            loadedChunks.Add(qSE);
        }
        if (!loadedChunks.Contains(qN))// && playerPos.y > qInit.y + defaultChunkSize / 4)
        {
            //qN
            chunksToGenerate.Add(qN);
            loadedChunks.Add(qN);
        }
        if (!loadedChunks.Contains(qNW))// && playerPos.y > qInit.y + defaultChunkSize / 4 && playerPos.x < qInit.x - defaultChunkSize / 4)
        {
            //qNW
            chunksToGenerate.Add(qNW);
            loadedChunks.Add(qNW);
        }
        if (!loadedChunks.Contains(qS))// && playerPos.y < qInit.y - defaultChunkSize / 4)
        {
            //qS
            chunksToGenerate.Add(qS);
            loadedChunks.Add(qS);
        }
        if (!loadedChunks.Contains(qSW))// && playerPos.x < qInit.x - defaultChunkSize / 4 && playerPos.y < qInit.y - defaultChunkSize / 4)
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
        for (int x = boundsCenterX - size / 2; x < boundsCenterX + size / 2; x++)
        {
            for (int y = boundsCenterY - size / 2; y < boundsCenterY + size / 2; y++)
            {
                groundTilemap.SetTile(new Vector3Int(x, y, 0), null);
            }
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
    private int sd;
    private float amp;
    private float freq;
    private float pers;
    private float lac;
    private int oct;
    public NativeArray<float> noiseValues;
    public GenerateMapJob(int boundsCenterX, int boundsCenterY, int size, NoiseParameters nos, NativeArray<float> nosValues)
    {
        bX = boundsCenterX;
        bY = boundsCenterY;
        sz = size;
        sd = nos.sd;
        amp = nos.amp;
        freq = nos.freq;
        pers = nos.pers;
        lac = nos.lac;
        oct = nos.oct;
        noiseValues = nosValues;
    }
    public void Execute()
    {
        //noiseValues = new float[sz * sz];
        float max = 0f;
        float min = float.MaxValue;
        int index = 0;
        for (int i = bX - sz / 2; i < bX + sz / 2; i++)
        {
            for (int j = bY - sz / 2; j < bY + sz / 2; j++)
            {
                float tempA = amp;
                float tempF = freq;
                noiseValues[index] = 0f;
                for (int k = 0; k < oct; k++)
                {
                    noiseValues[index] += Mathf.PerlinNoise(((i + sd) / (float)sz * freq), (j + sd) / (float)sz * freq) * amp;
                    freq *= lac;
                    amp *= pers;
                }
                amp = tempA;
                freq = tempF;
                if (noiseValues[index] > max) max = noiseValues[index];
                if (noiseValues[index] < min) min = noiseValues[index];
                index++;
            }
        }
        index = 0;
        for (int i = 0; i < sz; i++)
        {
            for (int j = 0; j < sz; j++)
            {
                noiseValues[index] = Mathf.InverseLerp(max, min, noiseValues[index]);
            }
        }
        
    }
}