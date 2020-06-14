using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]//[CreateAssetMenu(fileName = "New Transition Condition",menuName = "Custom/AI/Transition Conditions/")]
public class ActorStateTransitionCondition : ScriptableObject
{
    public virtual bool Accept(GameObject actor)
    {
        return false;
    }
}
