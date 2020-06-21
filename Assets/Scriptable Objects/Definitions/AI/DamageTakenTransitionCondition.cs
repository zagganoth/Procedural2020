using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage Taken", menuName = "Custom/AI/Transition Conditions/Damage Taken")]
public class DamageTakenTransitionCondition : ActorStateTransitionCondition
{
    EnemyStateMachine enem;
    public override void Initialize(GameObject actor)
    {
        enem = actor.GetComponent<EnemyStateMachine>();
    }

    public override bool Accept()
    {
        //Debug.Log("Damage transcond: " + enem.tokens.ContainsKey("damage"));
        return enem.tokens.ContainsKey("damage");
    }
}
