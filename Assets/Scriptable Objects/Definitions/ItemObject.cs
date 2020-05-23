using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Item", menuName = "Custom/Inventory System/Item")]
public class ItemObject : ScriptableObject
{
    public Sprite image;
    public int maxStackSize;
    public int count;

    public virtual void subscribeToEvents()
    {

    }
    private void Awake()
    {
        maxStackSize = 1;   
    }
    public virtual bool canSwing()
    {
        return false;
    }
    public virtual bool canBePlaced()
    {
        return false;
    }
    public virtual void use(Vector3 location, Tilemap tim, PlayerController playerRef)
    {

    }
}
