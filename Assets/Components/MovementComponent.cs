using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct MovementComponent : IComponentData
{
    public float moveSpeed;
    public float moveX;
    public float moveY;
}
