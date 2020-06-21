using System;
using System.Collections;
using System.Collections.Generic;
//This code is largely from https://github.com/m3rt32/NodeBasedDialogueSystem
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ActorStateNode : Node
{
    public string GUID;

    public ActorState relevantState;

    public string nodeName;

    public Dictionary<ActorStateTransitionCondition, ActorState> ports = new Dictionary<ActorStateTransitionCondition, ActorState>();

    public bool EntryPoint;

}
#endif