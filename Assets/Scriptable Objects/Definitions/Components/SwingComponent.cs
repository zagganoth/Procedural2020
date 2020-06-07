using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class SwingComponent : ItemComponent
{
    [SerializeField]
    public float swingTime;
    public int range;
    [SerializeField]
    public AnimatorController animator;
}
