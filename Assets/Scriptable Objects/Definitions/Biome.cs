using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Biome", menuName = "Custom/Worldgen/Biome")]
public class Biome : ScriptableObject
{
    [SerializeField]
    public List<tilemapData> tilemaps;
    [SerializeField]
    public List<Structure> possibleStructures;
    [SerializeField]
    public float minMoisture;
    [SerializeField]
    public float maxMoisture;

    public float spawnOdds;
    /*
     * To add in later: another noise field 
     * [SerializeField]
     * public float minMagic*/
}
[Serializable]
public class tilemapData
{
    [SerializeField]
    public int tilemapIndex;
    [SerializeField]
    public List<heightmapTile> tiles;
    public TileBase getTileForHeight(float heightNoiseValue)
    {
        foreach(var tile in tiles)
        {
            if(heightNoiseValue >= tile.minTileHeight && heightNoiseValue <= tile.maxTileHeight)
            {
                return tile.tile;
            }
        }
        return null;
    }

}
[Serializable]
public class heightmapTile
{
    public float minTileHeight;
    public float maxTileHeight;
    public float tileProbability;
    public TileBase tile;
}