using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
[Serializable]
public abstract class BaseInventoryUIComponents : MonoBehaviour
{
    public List<DragButton> slots;
    public List<Image> slotImages;
    public List<Image> slotBackgrounds;
    public List<Text> slotCounts;

    protected abstract void childConstructor();
    public abstract Inventory GetRelevantInventory();
    [SerializeField]
    protected Inventory activeInventory;
    private DraggableObject dragStance;
    protected bool active;
    protected GameObject parent;
    protected virtual void childUIUpdateStep(int index) { }
    /*[SerializeField]
    protected HashSet<InputControl> relevantActions;*/
    public void setRelevantVariables()
    {

        //invstance.SetActiveInventory(GetRelevantInventory());
        slotImages = new List<Image>();
        slotBackgrounds = new List<Image>();
        slots = new List<DragButton>(GetComponentsInChildren<DragButton>());
        slotCounts = new List<Text>();
        Image[] childComponents;
        int index = 0;
        foreach (var slot in slots)
        {
            slot.slotIndex = index;
            childComponents = slot.GetComponentsInChildren<Image>(includeInactive: true);
            slotImages.Add(childComponents[1]);
            slotBackgrounds.Add(childComponents[0]);
            slotCounts.Add(slot.GetComponentInChildren<Text>(includeInactive: true));
            index++;
        }
        
    }
   
    protected virtual void Start()
    {
        dragStance = DraggableObject.instance;
        active = true;
        //relevantActions = new HashSet<InputControl>();
        childConstructor();
        setRelevantVariables();
        parent = gameObject.transform.parent.gameObject;
        //EventManager.instance.OnInventoryAction += handleKeyEvent;
    }
    public virtual bool AddItemToInventory(ItemObject item)
    {
        if (activeInventory.AddItem(item))
        {
            updateUI();
            return true;
        }
        return false;
    }
    protected virtual void updateUI()
    {
        int index = 0;
        foreach (var slot in slotImages)
        {
            if (index >= activeInventory.getSize()
                || activeInventory.items[index] == null)
            {
                slot.gameObject.SetActive(false);
                slotCounts[index].gameObject.SetActive(false);
            }
            else
            {
                slot.gameObject.SetActive(true);
                slot.sprite = activeInventory.items[index].image;
                if(activeInventory.items[index].maxStackSize > 1) slotCounts[index].gameObject.SetActive(true);
                slotCounts[index].text = activeInventory.itemCounts[index].ToString();
            }
            childUIUpdateStep(index);
            index++;
        }
    }
    protected void ToggleParent()
    {
        //if(parent ==null) parent = gameObject.transform.parent.gameObject;
        parent.SetActive(!parent.activeSelf);
        active = parent.activeSelf;
    }
    public virtual void slotClicked(int itemIndex)
    {
        if (dragStance.item != null)
        {
            if (activeInventory.items[itemIndex] == null)
            {
                activeInventory.items[itemIndex] = dragStance.item;
            }
            dragStance.Reset();
            dragStance.dragging = false;

        }//must be starting drag on item
        else if (activeInventory.items[itemIndex] != null)
        {
            dragStance.dragging = true;
            dragStance.gameObject.SetActive(true);
            dragStance.img.sprite = activeInventory.items[itemIndex].image;
            dragStance.item = activeInventory.items[itemIndex];
            activeInventory.items[itemIndex] = null;
        }
        updateUI();
    }
}