using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Item")]
public class ItemObject : ScriptableObject
{
    public Sprite image;
    public int maxStackSize;
    public int count;
    private void Awake()
    {
        maxStackSize = 1;   
    }
    public virtual bool canSwing()
    {
        return false;
    }
}
