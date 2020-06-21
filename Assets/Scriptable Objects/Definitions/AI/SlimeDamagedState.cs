using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damaged", menuName = "Custom/AI/States/Slime Damaged")]
public class SlimeDamagedState : ActorState
{
    Animator anim;
    healthbar health;
    EnemyStateMachine enemState;
    protected override void InitializeVars()
    {
        anim = curObject.GetComponent<Animator>();
        anim.SetTrigger("Hurt");
        health = curObject.GetComponentInChildren<healthbar>();
        enemState = curObject.GetComponent<EnemyStateMachine>();
        if(enemState.tokens.ContainsKey("damage"))
            health.takeDamage((float)enemState.tokens["damage"]);
    }

    protected override void ChildExecute()
    {
    }


    protected override void WrapUp()
    {
        //Debug.Log("Wrapping up damaged");
        enemState.tokens.Remove("damage");
    }
}
