using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
[GenerateAuthoringComponent]
public class InventoryComponent : IComponentData
{
    public Inventory inventory;
}
