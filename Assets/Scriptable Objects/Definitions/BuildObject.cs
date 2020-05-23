using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu(fileName = "New Buildable", menuName = "Custom/Inventory System/Building Block")]
public class BuildObject : ItemObject
{
    [SerializeField]
    private TileBase tile;
    public override void use(Vector3 location, Tilemap tim, PlayerController playerRef)
    {
        Vector3Int floorLoc = Vector3Int.FloorToInt(location);
        float dist = Vector3.Distance(playerRef.gameObject.transform.position, floorLoc);
        dist -= 10;
        if (dist < playerRef.placeRange)
        tim.SetTile(floorLoc,tile);
    }
    /*
    public override void subscribeToEvents()
    {
        //EventManager.instance.OnItemAddedToInventory += SavePlayerRef;
    }
    public void SavePlayerRef(object sender, EventManager.OnItemAddedToInventoryArgs e)
    {
        //playerRef = e.playerRef;
    }*/
    public override bool canBePlaced()
    {
        return true;
    }
}
