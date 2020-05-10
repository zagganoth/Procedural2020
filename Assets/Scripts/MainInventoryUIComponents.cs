using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Main Inventory UI", menuName = "Inventory System/UI/MainInventoryUI")]
public class MainInventoryUIComponents : BaseInventoryUIComponents
{
    protected override void childConstructor()
    {
        inventoryLayout = UIStance.mainInventory;
    }
    protected override Inventory GetRelevantInventory()
    {
        return invstance.activeInventories[invstance.playerInventoryIndex];
    }
    /*
    protected override void runInventoryAction()
    {

    }*/
}
