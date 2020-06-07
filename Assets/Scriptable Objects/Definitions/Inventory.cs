using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "New Inventory", menuName="Custom/Inventory System/Inventory")]
public class Inventory : MonoBehaviour
{
    [SerializeField]
    public List<ItemObject> items;
    [SerializeField]
    public List<int> itemCounts;
    [SerializeField]
    private int size;
    private int lastUsedIndex;
    private int usedSlots;
    private bool full;
    public void initialize(int relSize = -1)
    {
        if (relSize != -1) size = relSize;
        lastUsedIndex = 0;
        usedSlots = 0;
        items = new List<ItemObject>();
        itemCounts = new List<int>();
        initializeInventory();
    }
    public bool isFull()
    {
        return false;
    }
    private void initializeInventory()
    {
        for(var i = 0; i < size;i++)
        {
            items.Add(null);
            itemCounts.Add(1);
        }

    }
    public int getSize()
    {
        return size;
    }
    private int getFirstUnusedIndex()
    {
        int index = 0;
        foreach (ItemObject item in items)
        {
            if (item == null) return index;
            index++;
        }
        return -1;
    }
    public bool AddItem(ItemObject item, int position=-1)
    {
        //position -1 means end of list
        if (position > size) return false;
        int firstUnusedIndex = getFirstUnusedIndex();
        if (firstUnusedIndex == -1) return false;
        //Cannot stack, so return
        if (item.maxStackSize <= 1 && usedSlots >= size) return false;
        int lIndex = items.IndexOf(item);
        if (lIndex == -1)
        {
            //items.Add(item);
            if (position == -1)
                items[firstUnusedIndex] = item;
            else items[position] =  item;
        }
        else
        {
            if (itemCounts[lIndex] < items[lIndex].maxStackSize) itemCounts[lIndex]++;
            else
            {
                if (position == -1)
                    items[firstUnusedIndex] = item;
                else items[position] = item;
            }
        }
        usedSlots++;
        return true;
    }
    public void RemoveItem(ItemObject item)
    {
        if (items.Count == 0) return;
        int lIndex = items.IndexOf(item);
        if (lIndex != -1 && itemCounts[lIndex] > 1)
        {
            itemCounts[lIndex] -= 1;
        }
        else
        {
            items[lIndex] = null;
        }
        usedSlots--;
    }
}
