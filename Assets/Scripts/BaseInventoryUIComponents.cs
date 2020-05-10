using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public abstract Inventory GetRelevantInventory();
    protected abstract void runInventoryAction();
    [SerializeField]
    protected HashSet<InputControl> relevantActions;
    public void setRelevantVariables()
    {

        //invstance.SetActiveInventory(GetRelevantInventory());
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
    public virtual void Start()
    {
        invstance = InventoryManager.instance;
        UIStance = UIManager.instance;
        relevantActions = new HashSet<InputControl>();
        childConstructor();
        setRelevantVariables();
        EventManager.instance.OnInventoryAction += handleKeyEvent;
    }
    public void handleKeyEvent(object sender, EventManager.OnInventoryActionArgs e)
    {
        if(relevantActions.Contains(e.context.control))
            runInventoryAction();
    }
}