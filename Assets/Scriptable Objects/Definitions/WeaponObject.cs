using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Custom/Inventory System/Weapon")]
public class WeaponObject : ToolObject
{
    public int damage;
}
