using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable][CreateAssetMenu(fileName = "Always Accept Transition Condition", menuName = "Custom/AI/Transition Conditions/True")]
public class AlwaysTrueTransitionCondition : ActorStateTransitionCondition
{
    public override bool Accept()
    {
        return true;
    }
}
