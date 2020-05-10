using System.Collections;
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
    protected override void Start()
    {
        base.Start();
    }
    public override Inventory GetRelevantInventory()
    {
        return activeInventory;
    }
    
    public void OnInventoryOpen(InputAction.CallbackContext context)
    {
        Start();
        if (!context.performed) return;
        ToggleParent();
    }
}
