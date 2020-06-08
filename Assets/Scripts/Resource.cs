using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : LeftClickable
{
    [SerializeField]
    public ItemObject dropItem;

    public void CollectItem(EventManager.OnLeftClickArgs lef)
    {
        lef.invRef.AddItemToInventory(dropItem);
    }
}
