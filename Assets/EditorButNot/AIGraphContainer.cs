using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AIGraphContainer : ScriptableObject
{
    public List<ActorStateNodeLinkData> nodeLinks = new List<ActorStateNodeLinkData>();
    public List<ActorStateNodeData> nodeData = new List<ActorStateNodeData>();
}
