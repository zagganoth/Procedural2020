using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField]
    public DraggableObject draggable;
    [SerializeField]
    Sprite activeSlotSprite;
    [SerializeField]
    Sprite defaultSlotSprite;
    [SerializeField]
    public Inventory activeInventory;
    [SerializeField]
    List<ItemObject> currentInvItems;
    public int activeSlotIndex;
    public bool dragging;
    [SerializeField]
    private BaseInventoryUIComponents hotbarUI;
    [SerializeField]
    private BaseInventoryUIComponents mainInventoryUI;
    public BaseInventoryUIComponents activeInventoryUI;
    private void Awake()
    {
        if(instance != null && instance!=this)
        {
            return;
        }
        instance = this;
    }
    private void Start()
    {
        activeSlotIndex = 0;
        dragging = false;
        StartCoroutine(delayedStart());
    }
    IEnumerator delayedStart()
    {
        //Using this to ensure that the UI is always updated AFTER the inventory is updated with the item
        yield return new WaitForSeconds(0.2f);
        SetHotBarActive();
        EventManager.instance.OnItemAddedToInventory += updateInventoryUIEventHandler;
        //EventManager.instance.OnInventoryChanged += toggleFullInventoryEventHandler;
    }
    public void SetHotBarActive()
    {
        SetActiveInventoryUI(hotbarUI);
    }
    
    public void swapActiveInventory()
    {
        if (activeInventoryUI == hotbarUI)
        {
            SetActiveInventoryUI(mainInventoryUI);
        }
        else
        {
            SetActiveInventoryUI(hotbarUI);
        }

    }
    public void SetActiveInventoryUI(BaseInventoryUIComponents inventoryUI,ChestManager chest = null)
    {
        InventoryManager invstance = InventoryManager.instance;
        activeInventoryUI = inventoryUI;
        activeInventory = activeInventoryUI.GetRelevantInventory();
        invstance.SetActiveInventory(activeInventory);
        updateInventoryUI();
    }
    private void SetActiveSlotUI(int index)
    {
        if (index == activeSlotIndex)
        {
            activeInventoryUI.slotBackgrounds[index].sprite = activeSlotSprite;
        }
        else
        {
            activeInventoryUI.slotBackgrounds[index].sprite = defaultSlotSprite;
        }
    }
    public void updateInventoryUI()
    {
        int index = 0;
        bool usingHotbar = InventoryManager.instance.UsingHotBar();
        foreach (var slot in activeInventoryUI.slotImages)
        {
            if (index >= activeInventory.getSize()
                || activeInventory.items[index].image == null)
            {
                slot.gameObject.SetActive(false);
            }
            else
            {
                slot.gameObject.SetActive(true);
                slot.sprite = activeInventory.items[index].image;
            }
            if (usingHotbar) SetActiveSlotUI(index);
            index++;
        }
    }
    public void OnScroll(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        SetActiveInventoryUI(hotbarUI);
        Vector2 input = context.ReadValue<Vector2>();
        if(input.y > 0)
        {
            activeSlotIndex = ((activeSlotIndex + 1) % activeInventory.getSize());
        }
        else if(input.y < 0)
        {
            activeSlotIndex = ((activeSlotIndex - 1) % activeInventory.getSize());
        }
        if(activeSlotIndex < 0)
        {
            activeSlotIndex = activeInventory.getSize() - 1;
        }
        updateInventoryUI();
    }
    
    public void updateInventoryUIEventHandler(object sender, EventManager.OnItemAddedToInventoryArgs e)//Inventory inventory)
    {
        updateInventoryUI();
    }

    public void ItemClicked(int itemIndex, BaseInventoryUIComponents parentInventory)
    {
        SetActiveInventoryUI(parentInventory);
        //if dragging an item currently
        if (draggable.item != null)
        {

            if (activeInventory.items[itemIndex].image == null)
            {
                activeInventory.items[itemIndex] = draggable.item;
            }
            draggable.Reset();
            dragging = false;

        }//must be starting drag on item
        else if(activeInventory.items[itemIndex].image != null)
        {
            dragging = true;
            draggable.gameObject.SetActive(true);
            draggable.img.sprite = activeInventory.items[itemIndex].image;
            draggable.item = activeInventory.items[itemIndex];
            activeInventory.items[itemIndex] = InventoryManager.instance.empty;
        }
        updateInventoryUI();
    }
}
