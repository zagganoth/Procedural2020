using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AINodePortData
{
    public int priority;
    public ActorStateTransitionCondition cond;
    public string portName;
    public ActorState destState;
}
