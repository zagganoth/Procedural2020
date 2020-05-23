﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainInventoryUIComponents : BaseInventoryUIComponents
{
    protected override void childConstructor()
    {
        activeInventory.initialize();
    }

    private void Awake()
    {
        EventManager.instance.OnInventoryAction += OnInventoryOpen;
    }
    protected override void Start()
    {
        base.Start();
        ToggleParent();
    }
    public override Inventory GetRelevantInventory()
    {
        return activeInventory;
    }
    
    public void OnInventoryOpen(object sender, EventManager.OnInventoryActionArgs e)//InputAction.CallbackContext context)
    {
        ToggleParent();
    }
}
