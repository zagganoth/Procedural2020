using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestManager : MonoBehaviour
{
    public Inventory chestInventory;
    /*[SerializeField]
    List<ItemObject> inventoryItems;*/
    [SerializeField]
    int defaultChestInventorySize;
    [SerializeField]
    ChestInventoryUIComponents chestInventoryUI;
    private bool hovering;
    [SerializeField]
    public int chestId;
    private void OnMouseEnter()
    {
        hovering = true;
    }
    private void OnRightClick(object sender, EventArgs e)
    {
        if(hovering)
        {
            chestInventoryUI.ToggleChest(this);
        }
    }
    private void OnMouseExit()
    {
        hovering = false;
    }
    private void initializeChestInventory()
    {
        chestId = -1;
        chestInventory = ScriptableObject.CreateInstance<Inventory>();
        chestInventory.initialize(defaultChestInventorySize);
        //inventoryItems = chestInventory.items;
    }
    // Start is called before the first frame update
    void Start()
    {
        EventManager.instance.OnRightClick += OnRightClick;
        initializeChestInventory();
        hovering = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
