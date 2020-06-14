using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
//Largely from https://github.com/m3rt32/NodeBasedDialogueSystem/blob/master/com.subtegral.dialoguesystem/Editor/GraphSaveUtility.cs
public class AIGraphSerializer
{
    public ActorAI destObject;
    private AIGraphView _targetGraphView;
    private AIGraphContainer _containerCache;
    public List<Edge> Edges;
    public List<ActorStateNode> Nodes;
    public static AIGraphSerializer GetInstance(AIGraphView targetGraphView)
    {
        return new AIGraphSerializer
        {
            _targetGraphView = targetGraphView,
            Edges = targetGraphView.edges.ToList(),
            Nodes = targetGraphView.nodes.ToList().Cast<ActorStateNode>().ToList()
        };
    }

    public void UpdateVars(AIGraphView targetGraphView)
    {
        _targetGraphView = targetGraphView;
        Edges = targetGraphView.edges.ToList();
        Nodes = targetGraphView.nodes.ToList().Cast<ActorStateNode>().ToList();
    }
    
    public void SaveGraph(ActorAI destObject)
    {
        
        if (!Edges.Any() && !Nodes.Any()) return;
        var inst = ScriptableObject.CreateInstance<AIGraphContainer>();
        SaveEdges();

        List<AINodePortData> npd;
        //destObject.stateTransitions.Add(null, Nodes.Find(x => x.EntryPoint).outputContainer.ElementAt)
        foreach(var stateNode in Nodes)
        {
            npd = new List<AINodePortData>();
            _targetGraphView.ports.ForEach((x) =>
            {
                ActorStateNode an = ((ActorStateNode)x.node);
                ActorStateTransitionCondition cond = null;

                if (an.GUID == stateNode.GUID)
                {
                    for (int i = 0; i < x.contentContainer.childCount; i++)
                    {
                        if (x.contentContainer.ElementAt(i).GetType() == typeof(ObjectField))
                        {
                            cond = ((x.contentContainer.ElementAt(i) as ObjectField).value as ActorStateTransitionCondition);
                        }
                    }
                    if (cond != null)
                    {
                        npd.Add(new AINodePortData
                        {
                            cond = cond,
                            portName = x.portName,
                            
                        });
                    }
                }
            });
            inst.nodeData.Add(new ActorStateNodeData
            {
                GUID = stateNode.GUID,
                relevantState = stateNode.relevantState,
                position = stateNode.GetPosition().position,
                title = stateNode.nodeName,
                ports = npd
            });    
        }
        if (destObject.editorGraphContainer)
        {
            AssetDatabase.RemoveObjectFromAsset(destObject.editorGraphContainer);
        }
        AssetDatabase.AddObjectToAsset(inst, AssetDatabase.GetAssetPath(destObject));
        destObject.editorGraphContainer = inst;
        _containerCache = inst;
        UpdateVars(_targetGraphView);
        AssetDatabase.SaveAssets();
    }
    
    public void SaveEdges()
    {
        if (Edges.Any())
        {
            //var firstOutput = Edges.First(x => (x.input.node as ActorStateNode).EntryPoint);
            //firstOutput.output 
            //destObject.stateTransitions.Add()
            var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
            ActorStateNode inputNode;
            ActorStateNode outputNode;
            for (int i = 0; i < connectedPorts.Length; i++)
            {
                outputNode = connectedPorts[i].output.node as ActorStateNode;
                inputNode = connectedPorts[i].input.node as ActorStateNode;

                inst.nodeLinks.Add(new ActorStateNodeLinkData
                {
                    sourceNodeGuid = outputNode.GUID,
                    portName = connectedPorts[i].output.portName,
                    destNodeGuid = inputNode.GUID
                });
            }
        }
    }
    public void LoadGraph(ActorAI sourceObject)
    {
        if(_targetGraphView==null)
        {
            Debug.Log("No Graph view!");
            return;
        }
        if (_containerCache == sourceObject.editorGraphContainer) return;
        _containerCache = sourceObject.editorGraphContainer;
        if (!_containerCache) return;
        ClearGraph();
        CreateNodes();
        UpdateVars(_targetGraphView);
        ConnectNodes();


    }

    private void ClearGraph()
    {
        if (!Nodes.Any()) return;
        if(Edges.Any())
            Nodes.Find(x => x.EntryPoint).GUID = _containerCache.nodeLinks[0].sourceNodeGuid;
        foreach (var perNode in Nodes)
        {
            /*if (perNode.EntryPoint)
            {
                perNode.RefreshPorts();
                continue;
            }*/
            Edges.Where(x => x.input.node == perNode).ToList()
                .ForEach(edge => _targetGraphView.RemoveElement(edge));
            _targetGraphView.RemoveElement(perNode);
        }

    }

    private void CreateNodes()
    {
        ActorStateNode tempNode;
        List<AINodePortData> nodePorts;
        foreach (var node in _containerCache.nodeData)
        {
            if (node.GUID == "0")
            {
                tempNode = _targetGraphView.GenerateEntryPointNode(node.ports.First().cond);
                tempNode.SetPosition(new Rect(node.position, _targetGraphView.defaultNodeSize));
                _targetGraphView.AddElement(tempNode);
                continue;
            }
            tempNode = _targetGraphView.GenerateAINode(node.title);
            
            tempNode.GUID = node.GUID;
            tempNode.SetPosition(new Rect(node.position, _targetGraphView.defaultNodeSize));
            _targetGraphView.AddElement(tempNode);

            //nodePorts = _containerCache.nodeData.Where(x => x.GUID == node.GUID).ToList();
            nodePorts = node.ports;
            nodePorts.ForEach(x => _targetGraphView.AddConditionPort(tempNode,x.portName,x.cond));
            tempNode.RefreshPorts();
            tempNode.RefreshExpandedState();
        }
        /*

        if (Nodes.Any())
        {
            foreach(ActorStateNode node in Nodes)
            {
                _targetGraphView.AddElement(node);
            }

        }
        else
        {

        }*/
    }

    private void ConnectNodes()
    {
        /*
        if (!Edges.Any()) return;
        foreach(var edge in Edges)
        {

            _targetGraphView.Add(edge);
            edge.input.Connect(edge);
            edge.output.Connect(edge);

            _targetGraphView.Refresh();
        }*/
        for(var i = 0; i < Nodes.Count;i++)
        {

            if (!_containerCache.nodeLinks.Any(x => x.sourceNodeGuid == Nodes[i].GUID))
            {
                continue;
            }
            var connections = _containerCache.nodeLinks.Where(x => x.sourceNodeGuid == Nodes[i].GUID).ToList();
            for(int j = 0; j < connections.Count; j++)
            {
                var targetNodeGuid = connections[j].destNodeGuid;
                var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer.ElementAt(0));
                targetNode.RefreshExpandedState();
                targetNode.RefreshPorts();
                //targetNode.SetPosition(new Rect(_containerCache.nodeData.First(x => x.GUID == targetNodeGuid).position, _targetGraphView.defaultNodeSize));
            }
        }
    }

    private void LinkNodes(Port output, Port input)
    {
        var tempEdge = new Edge()
        {
            output = output,
            input = input
        };

        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
        
        _targetGraphView.Add(tempEdge);
    }
}
#endif