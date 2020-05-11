using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Tol", menuName = "Inventory System/Item")]
public abstract class ToolObject : ItemObject
{
    public int range;
    public override bool canSwing()
    {
        return true;
    }
}
