using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventoryUI : MonoBehaviour
{
    public List<BaseInventoryUIComponents> playerInventoryUIs;
    private HotbarUIComponents hotbar;
    private void Start()
    {
        //base.Start();
        playerInventoryUIs = new List<BaseInventoryUIComponents>(GetComponentsInChildren<BaseInventoryUIComponents>());
        hotbar = GetComponentInChildren<HotbarUIComponents>();
        EventManager.instance.OnItemAddedToInventory += updateInventoryUIEventHandler;
    }
    public ItemObject GetActiveItem()
    {
        return hotbar.GetActiveItem();
    }
    private void updateInventoryUIEventHandler(object sender, EventManager.OnItemAddedToInventoryArgs e)
    {
        AddItemToInventory(e.item);
    }
    public bool AddItemToInventory(ItemObject item)
    {
        foreach(var inventoryUI in playerInventoryUIs)
        {
            if(inventoryUI.AddItemToInventory(item))
            {
                return true;
            }
        }
        return false;
    }
}
