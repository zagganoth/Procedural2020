using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    Sprite defaultSprite;
    [SerializeField]
    Sprite openSprite;
    SpriteRenderer sr;
    private void OnMouseEnter()
    {
        hovering = true;
    }
    private void OnRightClick(object sender, EventArgs e)
    {
        if(hovering)
        {
            chestInventoryUI.ToggleChest(this);
            if (sr.sprite != openSprite)
                sr.sprite = openSprite;
            else
                sr.sprite = defaultSprite;
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
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        defaultSprite = sr.sprite;
    
    }
    // Start is called before the first frame update
    void Start()
    {
        if(chestInventoryUI == null)
        {
            chestInventoryUI = UIParent.instance.GetComponentInChildren<ChestInventoryUIComponents>(includeInactive:true);
        }
        EventManager.instance.OnRightClick += OnRightClick;
        initializeChestInventory();
        hovering = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
