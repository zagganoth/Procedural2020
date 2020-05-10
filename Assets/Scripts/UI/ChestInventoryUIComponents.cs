using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChestInventoryUIComponents : BaseInventoryUIComponents
{
    private int chestId;
    private List<ChestManager> chests;
    //private ChestManager activeChest;
    protected override void childConstructor()
    {
        chests = new List<ChestManager>();
        //active = false;

    }
    public override Inventory GetRelevantInventory()
    {
        return activeInventory;//invstance.activeInventories[chestId];
    }
    protected override void Start()
    {
        base.Start();
        //ToggleParent();
    }
    private int AddChest(ChestManager chest)
    {
        chests.Add(chest);
        return chests.Count - 1;
    }
    public void ToggleChest(ChestManager chest)
    {
        Start();
        if(chest.chestId == -1)
        {
            chestId = AddChest(chest);
            chest.chestId = chestId;
        }
        activeInventory = chest.chestInventory;
        ToggleParent();

    }
}
