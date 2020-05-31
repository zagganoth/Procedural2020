using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Tol", menuName = "Inventory System/Item")]
public abstract class ToolObject : ItemObject
{
    public int range;
    [SerializeField]
    public AnimatorController animator;

}
