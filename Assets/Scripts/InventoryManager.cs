using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public static InventoryManager instance;
    [SerializeField]
    public List<Inventory> activeInventories;
    [SerializeField]
    public int playerInventoryIndex;
    [SerializeField]
    public int playerHotBarIndex;
    [SerializeField]
    int currentInventoryIndex;
    [SerializeField]
    public ItemObject empty;
    public int lastActiveChest;
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        currentInventoryIndex = playerHotBarIndex;
        activeInventories[playerInventoryIndex].initialize();
        activeInventories[currentInventoryIndex].initialize();
       
    }
    public void SetActiveInventory(Inventory inv)
    {
        int index = 0;
        foreach(var inven in activeInventories)
        {
            if(inven==inv)
            {
                currentInventoryIndex = index;
                return;
            }
            index++;
        }
        /*
        activeInventories.Add(inv);
        currentInventoryIndex = activeInventories.Count - 1;*/
        //UIManager.instance.SetActiveInventoryUI(UIManager.instance.mainInventory);
        //UIManager.instance.updateInventoryUI();
    }
    public void SetHotbarActive()
    {
        currentInventoryIndex = playerHotBarIndex;
    }
    public void SetMainInvActive()
    {
        currentInventoryIndex = playerInventoryIndex;
    }
    public int GetChestInventory(Inventory inv)
    {
        int index = 0;
        foreach (var inven in activeInventories)
        {
            if (inven == inv)
            {
                return index;
            }
            index++;
        }
        activeInventories.Add(inv);
        lastActiveChest = activeInventories.Count - 1;
        return activeInventories.Count - 1;
    }
    public bool UsingHotBar()
    {
        return currentInventoryIndex == playerHotBarIndex;
    }
    public Inventory getActiveInventory()
    {
        return activeInventories[currentInventoryIndex];
    }
    private void Start()
    {
        EventManager.instance.OnItemAddedToInventory += AddToInventory;
        //EventManager.instance.OnInventoryChanged += swapActivePlayerInventory;
        //EventManager.instance.OnItemAddedToInventory += UIManager.instance.updateInventoryUIEventHandler;

    }
    private void swapActivePlayerInventory(object sender, EventArgs e)
    {
        if (currentInventoryIndex == playerHotBarIndex) currentInventoryIndex = playerInventoryIndex;
        else if (currentInventoryIndex == playerInventoryIndex) currentInventoryIndex = playerHotBarIndex;
    }
    public void AddToInventory(object sender, EventManager.OnItemAddedToInventoryArgs e)
    {
        if (currentInventoryIndex > 1)
        {
            if (!activeInventories[playerHotBarIndex].isFull())
                currentInventoryIndex = playerHotBarIndex;
            else if (!activeInventories[playerInventoryIndex].isFull())
                currentInventoryIndex = playerInventoryIndex;
            else //all inventories full, do not add
                return;
        }
        
        if (UIManager.instance.activeInventory != activeInventories[currentInventoryIndex])
        {
            UIManager.instance.swapActiveInventory();
        }
        activeInventories[currentInventoryIndex].AddItem(e.item);
    }
    public void AddInventory(Inventory inventory)
    {
        activeInventories.Add(inventory);
    }
    public void RemoveInventory(Inventory inventory)
    {
        activeInventories.Remove(inventory);
    }
    public void SaveInventory(Inventory inventory)
    {
        return;
    }
}
