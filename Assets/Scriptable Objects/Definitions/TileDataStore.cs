using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "New TileDataStore", menuName = "Custom/Worldgen/TileDataStore")]
public class TileDataStore : ScriptableObject
{
    [SerializeField]
    public List<TileBase> tileList;

    public int GetIndexForTile(TileBase tile)
    {
        return tileList.IndexOf(tile);
    }
    public TileBase GetTileForIndex(int index)
    {
        return tileList[index];
    }
}
/*
[Serializable]
public class TileIndexData
{
    [SerializeField]
    public TileBase tile;
    [SerializeField]
    public int index;
}*/