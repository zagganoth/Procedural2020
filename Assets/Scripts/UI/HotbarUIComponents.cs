﻿using System;
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
    PlayerController playerRef;
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
        PlaceableComponent pc = activeInventory.items[activeSlotIndex].getComponent("PlaceableComponent") as PlaceableComponent;
        if (pc != null)//.canBePlaced())
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
