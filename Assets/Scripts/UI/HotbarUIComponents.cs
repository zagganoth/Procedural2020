using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public class HotbarUIComponents : BaseInventoryUIComponents
{
    int activeSlotIndex;
    protected Sprite defaultSlotSprite;
    [SerializeField]
    Sprite activeSlotSprite;
    [SerializeField]
    Tilemap wallTilemap;
    [SerializeField]
    PlacingUI placeUI;
    PlayerController playerRef;
    /*[SerializeField]
    EquippedTool equippedItem;*/
    protected override void childConstructor()
    {
        activeSlotIndex = 0;
        activeInventory.initialize();
        playerRef = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }
    protected override void Start()
    {
        base.Start();
        defaultSlotSprite = slotBackgrounds[0].sprite;
        slotBackgrounds[activeSlotIndex].sprite = activeSlotSprite;
        EventManager.instance.OnRightClick += placeBlock;
        updateUI();
    }
    public void placeBlock(object sender, EventManager.OnRightClickArgs e)
    {
        if (activeInventory.items[activeSlotIndex] == null) return;
        PlaceableComponent pc;
        if ((pc = activeInventory.items[activeSlotIndex].getComponent("PlaceableComponent") as PlaceableComponent) && !wallTilemap.GetTile(Vector3Int.FloorToInt(e.clickPosition)))
        {
            wallTilemap.SetTile(Vector3Int.FloorToInt(e.clickPosition), pc.GetRelevantTile());
            wallTilemap.RefreshAllTiles();
    }
        }
    public override Inventory GetRelevantInventory()
    {
        return activeInventory;
    }
    public ItemObject GetActiveItem()
    {
        return activeInventory.items[activeSlotIndex];
    }
    protected override void updateUI()
    {
        base.updateUI();
        CursorComponent uc;
        if (GetActiveItem() != null && (uc = GetActiveItem().getComponent("CursorComponent") as CursorComponent))
        {
            placeUI.gameObject.SetActive(true);
            placeUI.updateUIComponent(uc);
        }
        else placeUI.gameObject.SetActive(false);
    }
    
    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        MeleeComponent wc;
        if( activeInventory.items[activeSlotIndex] != null && (wc = activeInventory.items[activeSlotIndex].getComponent("MeleeComponent") as MeleeComponent))
        {
            float range = playerRef.interactRange;
            RangeOverrideComponent rc;
            if (rc = GetActiveItem().getComponent("RangeOverrideComponent") as RangeOverrideComponent)
                range = rc.range;
            RaycastHit2D hit;

            var ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            if ((Vector2.Distance(playerRef.transform.position,ray) < range) && (hit = Physics2D.Raycast(ray, Vector2.zero)) && hit.transform.CompareTag("Enemy"))
            {
                EnemyAI enem = hit.transform.gameObject.GetComponent<EnemyAI>();
                enem.takeDamage(wc.damage);
            }
        }
    }
    public void OnScroll(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Vector2 input = context.ReadValue<Vector2>();
        slotBackgrounds[activeSlotIndex].sprite = defaultSlotSprite;
        if (input.y > 0)
        {
            activeSlotIndex = ((activeSlotIndex + 1) % activeInventory.getSize());
        }
        else if (input.y < 0)
        {
            activeSlotIndex = ((activeSlotIndex - 1) % activeInventory.getSize());
        }
        if (activeSlotIndex < 0)
        {
            activeSlotIndex = activeInventory.getSize() - 1;
        }
        slotBackgrounds[activeSlotIndex].sprite = activeSlotSprite;
        updateUI();
    }
}
