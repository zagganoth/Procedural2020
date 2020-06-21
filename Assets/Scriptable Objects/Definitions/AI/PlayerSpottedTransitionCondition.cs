using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Spotted", menuName = "Custom/AI/Transition Conditions/Player Spotted")]
public class PlayerSpottedTransitionCondition : ActorStateTransitionCondition
{
    Animator anim;
    GameObject _actor;
    public override void Initialize(GameObject actor)
    {
        anim = actor.GetComponent<Animator>();
        _actor = actor;
    }
    [SerializeField]
    float spotRadius;
    public override bool Accept()
    {

        foreach (var collision in Physics2D.OverlapCircleAll(_actor.transform.position, spotRadius))
        {
            if (collision.CompareTag("Player"))
            {
                stateParams = collision.gameObject;
                anim.SetTrigger("InRange");
                return true;
            }
        }
        return false;
    }
}
