using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public abstract class BaseInventoryUIComponents : MonoBehaviour
{
    public List<DragButton> slots;
    public List<Image> slotImages;
    public List<Image> slotBackgrounds;
    public LayoutGroup inventoryLayout;
    protected InventoryManager invstance;
    protected UIManager UIStance;
    protected abstract void childConstructor();
    protected abstract Inventory GetRelevantInventory();
    //protected abstract void runInventoryAction();
    public void setRelevantVariables()
    {
        invstance.SetActiveInventory(GetRelevantInventory());
        slotImages = new List<Image>();
        slotBackgrounds = new List<Image>();
        slots = new List<DragButton>(GetComponentsInChildren<DragButton>());
        Image[] childComponents;
        int index = 0;
        foreach (var slot in slots)
        {
            slot.slotIndex = index;
            childComponents = slot.GetComponentsInChildren<Image>(includeInactive: true);
            slotImages.Add(childComponents[1]);
            slotBackgrounds.Add(childComponents[0]);
            index++;
        }
    }
    public void Start()
    {
        invstance = InventoryManager.instance;
        UIStance = UIManager.instance;
        childConstructor();
        setRelevantVariables();
    }
    public void handleKeyEvent()
    {

    }
}