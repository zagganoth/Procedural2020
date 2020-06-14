using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable][CreateAssetMenu(fileName = "New Transition Condition",menuName = "Custom/AI/Transition Condition")]
public class ActorStateTransitionCondition : ScriptableObject
{

    bool Accept(GameObject actor)
    {
        return false;
    }
}
