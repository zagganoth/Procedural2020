using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Chest Inventory UI", menuName = "Inventory System/UI/ChestInventoryUI")]
public class ChestInventoryUIComponents : BaseInventoryUIComponents
{
    private int chestId;
    protected override void childConstructor()
    {
        inventoryLayout = GetComponent<LayoutGroup>();
    }
    public override Inventory GetRelevantInventory()
    {
        return invstance.activeInventories[chestId];
    }
    public override void Start()
    {
        base.Start();
        gameObject.transform.parent.gameObject.SetActive(false);
    }
    protected override void runInventoryAction()
    {
        throw new System.NotImplementedException();
    }
    public void ToggleChest(ChestManager chest)
    {
        if(chest.chestId == -1)
        {
            chestId = invstance.GetChestInventory(chest.chestInventory);
            chest.chestId = chestId;
        }
        GameObject parent = gameObject.transform.parent.gameObject;
        if (parent.activeSelf)
        {
            UIStance.SetHotBarActive();
            parent.SetActive(false);
        }
        else
        {
            parent.SetActive(true);
            UIStance.SetActiveInventoryUI(this);
        }
    }
}
