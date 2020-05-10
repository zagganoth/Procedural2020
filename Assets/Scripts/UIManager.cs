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
    public LayoutGroup hotbar;
    [SerializeField]
    public LayoutGroup mainInventory;
    [SerializeField]
    public LayoutGroup chestInventory;
    private List<DragButton> slots;
    private List<Image> slotImages;
    private List<Image> slotBackgrounds;
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
    public LayoutGroup activeInventoryUI;
    public int activeSlotIndex;
    public bool dragging;
    [SerializeField]
    private BaseInventoryUIComponents hotbarUI;
    private BaseInventoryUIComponents mainInventoryUI;
    private BaseInventoryUIComponents chestInventoryUI;
    public BaseInventoryUIComponents currentInventoryUI;
    private void Awake()
    {
        if(instance != null && instance!=this)
        {
            return;
        }
        instance = this;
        activeInventory = InventoryManager.instance.getActiveInventory();
        SetActiveInventoryUI(hotbar);

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
        EventManager.instance.OnItemAddedToInventory += updateInventoryUIEventHandler;
        //EventManager.instance.OnInventoryChanged += toggleFullInventoryEventHandler;
        currentInventoryUI = hotbarUI; 
    }
    public void toggleFullInventoryEventHandler(object sender, EventArgs e)
    {
        toggleFullInventory();
    }
    public void toggleFullInventory()
    {
        GameObject parent = mainInventory.transform.parent.gameObject;

        parent.SetActive(!parent.activeSelf);

        swapActiveInventory();
            
    }
    public void swapActiveInventory()
    {
        if (activeInventoryUI == hotbar)
        {
            SetActiveInventoryUI(mainInventory);
        }
        else if (activeInventoryUI == mainInventory)
        {
            SetActiveInventoryUI(hotbar);
        }
        else
        {
            SetActiveInventoryUI(hotbar);
        }
    }

    public void ToggleChestUIOpen(LayoutGroup chestInv,ChestManager chest = null)
    {
        GameObject parent = chestInventory.transform.parent.gameObject;
        if(parent.activeSelf)
        {
            parent.SetActive(false);
            SetActiveInventoryUI(hotbar);
        }
        else
        {
            parent.SetActive(true);
            SetActiveInventoryUI(chestInv, chest);
        }
    }
    public void SetActiveInventoryUI(LayoutGroup inventory,ChestManager chest = null)
    {
        InventoryManager invstance = InventoryManager.instance;
        activeInventoryUI = inventory;
        if (activeInventoryUI.GetInstanceID() == hotbar.GetInstanceID())
        {
            activeInventory = invstance.activeInventories[invstance.playerHotBarIndex];
        }
        else if (activeInventoryUI.GetInstanceID() == mainInventory.GetInstanceID())
        {
            activeInventory = invstance.activeInventories[invstance.playerInventoryIndex];
        }
        else if(activeInventoryUI.GetInstanceID() == chestInventory.GetInstanceID())
        {
            if (chest == null) activeInventory = invstance.activeInventories[invstance.lastActiveChest];
            else
            {
                if (chest.chestId == -1)
                {
                    int chestId = invstance.GetChestInventory(chest.chestInventory);
                    chest.chestId = chestId;
                }
                activeInventory = chest.chestInventory;
            }
        }
        invstance.SetActiveInventory(activeInventory);
        currentInvItems = activeInventory.items;
        slotImages = new List<Image>();
        slotBackgrounds = new List<Image>();
        slots = new List<DragButton>();
        //Button[] slots;
        slots.AddRange(inventory.GetComponentsInChildren<DragButton>());
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
        updateInventoryUI();
    }
    private void SetActiveSlotUI(int index)
    {
        if (index == activeSlotIndex)
        {
            slotBackgrounds[index].sprite = activeSlotSprite;
        }
        else
        {
            slotBackgrounds[index].sprite = defaultSlotSprite;
        }
    }
    public void updateInventoryUI()
    {
        int index = 0;
        bool usingHotbar = InventoryManager.instance.UsingHotBar();

        foreach (var slot in slotImages)
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
        SetActiveInventoryUI(hotbar);
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

    public void ItemClicked(int itemIndex, LayoutGroup parentInventory)
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
