using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
/*
public class CharacterMovementSystem : JobComponentSystem
{

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        JobHandle myJob = Entities.ForEach((ref Translation trans, in MovementComponent data) =>
        {
        trans.Value.x = trans.Value.x + data.moveX * data.moveSpeed * deltaTime;
        trans.Value.y = trans.Value.y + data.moveY * data.moveSpeed * deltaTime;
        }).Schedule(inputDeps);
        return myJob;
    }
}
*/