using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dead", menuName = "Custom/AI/Transition Conditions/Dead")]
public class DeadCondition : ActorStateTransitionCondition
{
    Animator anim;
    healthbar health;
    public override void Initialize(GameObject actor)
    {
        health = actor.GetComponentInChildren<healthbar>();
    }

    public override bool Accept()
    {
        return health.dead;
    }
}
