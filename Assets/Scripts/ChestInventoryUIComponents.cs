using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Chest Inventory UI", menuName = "Inventory System/UI/ChestInventoryUI")]
public class ChestInventoryUIComponents : BaseInventoryUIComponents
{
    protected override void childConstructor()
    {
        inventoryLayout = GetComponent<LayoutGroup>();
    }
    protected override Inventory GetRelevantInventory()
    {
        return invstance.activeInventories[invstance.lastActiveChest];
    }
}
