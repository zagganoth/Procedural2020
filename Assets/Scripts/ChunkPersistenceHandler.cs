using Google.Protobuf.Collections;
using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkPersistenceHandler : MonoBehaviour
{
    Dictionary<TileBase, int> indexForTile;
    [SerializeField]
    TileDataStore tileStore;
    private void Awake()
    {
        indexForTile = new Dictionary<TileBase, int>();
    }
    public void CreateAndSaveChunk(int centerX, int centerY, List<TileBase[]> tileList,HashSet<int> modifiedTilemaps)
    {
        try
        {
            List<intList> tileIndexList = new List<intList>();
            int index = 0;
            TileBase[] tileRay;
            foreach(var intdex in modifiedTilemaps)
            {
                tileRay = tileList[intdex];
                tileIndexList.Add(new intList());
                tileIndexList[index].tilemapIndex = intdex;
                foreach(var tile in tileRay)
                {
                    if (tile == null)
                    {
                        tileIndexList[index].list.Add(-1);
                        continue;
                    }
                    if(!indexForTile.ContainsKey(tile))
                    {
                        indexForTile.Add(tile, tileStore.GetIndexForTile(tile));
                    }
                    tileIndexList[index].list.Add(indexForTile[tile]);
                }
                index++;
            }
            Chunk chunk = new Chunk(centerX, centerY, tileIndexList);
        
            string dirPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/My Games/Procedural/" + centerX + "_" + centerY + ".cnk";
            using (FileStream stream = new FileStream(dirPath, FileMode.Create, FileAccess.Write))
            {
                Serializer.Serialize(stream, chunk);
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Exception!");
        }
        //Chunk chunk = new Chunk();
    }
    public DecompressedChunk LoadChunkFromDisk(int centerX, int centerY,int size)
    {

        string dirPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/My Games/Procedural/" + centerX + "_" + centerY + ".cnk";
        if(!File.Exists(dirPath))
        {
            return null;
        }
        else
        {
            //Debug.Log(dirPath);
        }
        Chunk chunk = Serializer.Deserialize<Chunk>(new FileStream(dirPath, FileMode.Open, FileAccess.Read));
        DecompressedChunk retVal = new DecompressedChunk(chunk.tileIndexesByTilemap.Count,size*size);
        //Debug.Log(chunk.tileIndexesByTilemap.Count);
        //for(int i = 0; i < )
        int index = 0;
        int ind2 = 0;
        foreach(var iList in chunk.tileIndexesByTilemap)
        {
            index = 0;
            /*retVal.positionLists.Add(new Vector3Int[size * size]);
            retVal.tileLists.Add(new TileBase[size * size]);*/
            for (int x = (centerX * size) - size / 2; x < (centerX * size) + size / 2; x++)
            {
                for (int y = (centerY * size) - size / 2; y < (centerY * size) + size / 2; y++)
                {
                    //Debug.Log(index);
                    retVal.positionLists[ind2][index] = new Vector3Int(x, y,0);
                    retVal.tileLists[ind2][index] = tileStore.GetTileForIndex(iList.list[index++]);
                }
            }
            retVal.tilemapIndices.Add(iList.tilemapIndex);
        }

            /*foreach(var iList in chunk.tileIndexesByTilemap)
            {
                retVal.tileLists.Add()
            }*/
        return retVal;
    }
}

public class DecompressedChunk
{
    public HashSet<int> tilemapIndices;
    public List<TileBase[]> tileLists;
    public List<Vector3Int[]> positionLists;
    public DecompressedChunk(int listSize, int arraySize)
    {
        tileLists = new List<TileBase[]>();
        for(int i = 0; i < listSize;i++)
        {
            tileLists.Add(new TileBase[arraySize]);
        }
        positionLists = new List<Vector3Int[]>();
        for (int i = 0; i < listSize; i++)
        {
            positionLists.Add(new Vector3Int[arraySize]);
        }
        tilemapIndices = new HashSet<int>();
    }
    
}

