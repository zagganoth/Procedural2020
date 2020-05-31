using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlaceableComponent : ItemComponent
{
    [SerializeField]
    public TileBase tile;
    public TileBase GetRelevantTile()
    {
        return null;
    }
}
