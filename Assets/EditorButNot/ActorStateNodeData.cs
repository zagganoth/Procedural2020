using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]
public class ActorStateNodeData
{
    public string GUID;
    public ActorState relevantState;
    public string title;
    public Vector2 position;
    public List<AINodePortData> ports;
    public bool EntryPoint = false;
}
