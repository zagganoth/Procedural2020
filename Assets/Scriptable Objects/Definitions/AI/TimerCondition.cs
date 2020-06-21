using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Timer", menuName = "Custom/AI/Transition Conditions/Timer")]
public class TimerCondition : ActorStateTransitionCondition
{
    private DateTime nextSaveTime;
    public override void Initialize(GameObject actor)
    {
        nextSaveTime = DateTime.Now.AddSeconds(1);
    }
    public override bool Accept()
    {
        return DateTime.Now >= nextSaveTime;
    }
}
