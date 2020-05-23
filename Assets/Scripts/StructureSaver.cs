using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StructureSaver : MonoBehaviour
{
    [SerializeField]
    int2 scanDimensions;
    [SerializeField]
    List<Tilemap> structuresTilemaps;
    [SerializeField]
    Structure destStructure;
    // Start is called before the first frame update
    void Start()
    {
        destStructure.tileLists = new List<TileList>();
        destStructure.positionLists = new List<Vector3List>();
        int index = 0;
        foreach (var tilemap in structuresTilemaps)
        {
            findStructureTiles(tilemap, index);
            index++;
        }
        EditorUtility.SetDirty(destStructure);
    }
    void findStructureTiles(Tilemap structureTilemap,int tilemapIndex)
    {
        destStructure.tileLists.Add(new TileList(new List<TileBase>()));
        destStructure.positionLists.Add(new Vector3List(new List<Vector3Int>()));
        //Debug.Log("Starting my work!");
        int minRowWithTile = int.MaxValue;
        int maxRowWithTile = int.MinValue;
        int minColWithTile = int.MaxValue;
        int maxColWithTile = int.MinValue;
        for (int j = -(scanDimensions.y/ 2); j < scanDimensions.y / 2; j++)
        {
            TileBase tile = null;
            for (int i = -(scanDimensions.x/ 2); i < scanDimensions.x/ 2;i++)
            {
                Vector3Int curPos = new Vector3Int(i, j, 0);
                tile = structureTilemap.GetTile(curPos);
                if(tile!=null)
                {
                    if(i < minColWithTile)
                    {
                        minColWithTile = i;
                    }
                    else if(i > maxColWithTile)
                    {
                        maxColWithTile = i;
                    }
                    if(j < minRowWithTile)
                    {
                        minRowWithTile = j;
                    }
                   if(j > maxRowWithTile)
                    {
                        maxRowWithTile = j;
                    }
                    destStructure.tileLists[tilemapIndex].list.Add(tile);
                    destStructure.positionLists[tilemapIndex].list.Add(curPos);
                }
            }
        }
    }
}
