using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Main Inventory UI", menuName = "Inventory System/UI/MainInventoryUI")]
public class MainInventoryUIComponents : BaseInventoryUIComponents
{
    protected override void childConstructor()
    {
        InputAction relevantAction = new InputMaster().Player.OpenInventory;
        foreach(var control in relevantAction.controls)
        {
            relevantActions.Add(control);
        }
        //inventoryLayout = UIStance.mainInventory;
    }
    public override void Start()
    {
        base.Start();
        gameObject.transform.parent.gameObject.SetActive(false);
    }
    public override Inventory GetRelevantInventory()
    {
        return invstance.activeInventories[invstance.playerInventoryIndex];
    }
    
    protected override void runInventoryAction()
    {
        GameObject parent = gameObject.transform.parent.gameObject;
        if (parent.activeSelf)
        {
            //invstance.SetHotbarActive();
            UIStance.SetHotBarActive();
            parent.SetActive(false);
        }
        else
        {

            parent.SetActive(true);
            //invstance.SetMainInvActive();
            UIStance.SetActiveInventoryUI(this);
        }
    }
}
