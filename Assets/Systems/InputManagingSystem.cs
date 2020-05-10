using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
/*
[AlwaysSynchronizeSystem]
public class InputManagingSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.ForEach((ref MovementComponent moveData, in PlayerMovementInputComponent inputData) =>
        {
            moveData.moveY = 0;
            moveData.moveX = 0;
            moveData.moveY += Input.GetKey(inputData.upKey) ? 1 : 0;
            moveData.moveY -= Input.GetKey(inputData.downKey) ? 1 : 0;
            moveData.moveX -= Input.GetKey(inputData.leftKey) ? 1 : 0;
            moveData.moveX += Input.GetKey(inputData.rightKey) ? 1 : 0;
        }).Run();
        return default;
    }

}
*/