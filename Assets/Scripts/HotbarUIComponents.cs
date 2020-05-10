using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Hotbar UI", menuName = "Inventory System/UI/HotbarUI")]
public class HotbarUIComponents : BaseInventoryUIComponents
{
    protected override void childConstructor()
    {
        inventoryLayout = GetComponent<LayoutGroup>();
    }
    public override Inventory GetRelevantInventory()
    {
        return invstance.activeInventories[invstance.playerHotBarIndex];
    }

    protected override void runInventoryAction()
    {
        throw new System.NotImplementedException();
    }
}
