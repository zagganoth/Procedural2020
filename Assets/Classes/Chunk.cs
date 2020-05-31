using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using ProtoBuf;

[Serializable]
[ProtoContract]
public class Chunk
{
    /*
    [SerializeField]
    public List<Vector3List> positionLists;*/
    [SerializeField][ProtoMember(1)]
    public List<intList> tileIndexesByTilemap;

    [ProtoMember(2)]
    public int cX;
    [ProtoMember(3)]
    public int cY;
    public Chunk()
    {

    }
    public Chunk(int centerX, int centerY, List<intList> tileList)
    {
        tileIndexesByTilemap = tileList;
        cX = centerX;
        cY = centerY;
        /*
        int index = 0;
        foreach (TileBase[] tileRay in tileList)
        {
            tileIndexesByTilemap.Add(new List<int>());
            foreach(var tile in tileRay)
            {
                if(!indexForTile.ContainsKey(tile))
                {
                    indexForTile.Add(tile,TileDataStore)
                }
            }
            index++;

        }*/
        //tileLists.Add(new TileList(tileList.ToList()));
        //List<Vector2Int> destList = new List<Vector2Int>();

        
        //foreach(TileBase[] tileRay in tileList)
        

    }
    /*
    public void SaveToFile()
    {
      
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(dirPath + cX + "_"+cY + ".sav",json);
        //Protobuf.


            
    }
    public static bool ChunkExists(int cX, int cY)
    {
        return File.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/My Games/Procedural/" + cX + "_" + cY + ".sav");
    }
    public static Chunk LoadFromFile(int cX, int cY)
    {
        
        string fPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/My Games/Procedural/" + cX + "_" + cY + ".sav";
        if (File.Exists(fPath))
        {
            Chunk chunk = new Chunk(cX, cY, new List<TileBase[]>(), new List<Vector3Int[]>());
            JsonUtility.FromJsonOverwrite(File.ReadAllText(fPath), chunk);
            return chunk;
        }
        return null;
    }
    public List<TileBase[]> getTileList()
    {
        
        List<TileBase[]> tileList = new List<TileBase[]>();
        int index = 0;
        int subIndex = 0;
        foreach(var list in tileLists)
        {

            tileList.Add(new TileBase[list.list.Count()]);
            subIndex = 0;
            foreach(var l2 in list.list)
            {
                tileList[index][subIndex++]= l2;
            }
            index++;
        }
        return null;
    }
    public List<Vector3Int[]> getPositionList()
    {
        
        List<Vector3Int[]> posList = new List<Vector3Int[]>();
        int index = 0;
        int subIndex = 0;
        foreach (var list in positionLists)
        {

            posList.Add(new Vector3Int[list.list.Count()]);
            subIndex = 0;
            foreach (Vector3Int l2 in list.list)
            {
                posList[index][subIndex++] = l2;
            }
            index++;
        }
        return null;
    }*/
}
[ProtoContract]
public class intList
{
    [ProtoMember(1)]
    public List<int> list;
    [ProtoMember(2)]
    public int tilemapIndex;

    public intList()
    {
        list = new List<int>();
    }
}
