using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Entities.UniversalDelegates;
using UnityEditor;
//This code is largely from https://github.com/m3rt32/NodeBasedDialogueSystem
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEditor.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class AIGraphView : GraphView
{
    public Vector2 defaultNodeSize = new Vector2(1000, 400);
    public ActorStateNode _entryPointNode;
    public AIGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("AIGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());


        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
        _entryPointNode = GenerateEntryPointNode();
        AddElement(_entryPointNode);
    }
    public void Refresh()
    {
        foreach(Node node in nodes.ToList())
        {
            node.RefreshExpandedState();
            node.RefreshPorts();
        }
    }
    private Port GeneratePort(ActorStateNode node, Direction portDirection, Port.Capacity cap=Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, cap, typeof(float));
    }

    public void CreateNode(string nodeName)
    {
        AddElement(GenerateAINode(nodeName));
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach((port) =>
        {
            if (startPort != port && startPort.node != port.node)
                compatiblePorts.Add(port);
        });
        return compatiblePorts;
    }

    public ActorStateNode GenerateAINode(string nodeName)
    {
        var retNode = new ActorStateNode
        {
            relevantState = null,
            GUID = Guid.NewGuid().ToString(),   
        };

        var oldLabel = retNode.titleContainer.Q<Label>();
        retNode.titleContainer.Remove(oldLabel);

        var inputPort = GeneratePort(retNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        retNode.inputContainer.Add(inputPort);
        retNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

        var button = new Button(() => { AddConditionPort(retNode); });
        button.text = "New State Transition Condition";

        if (nodeName == "New AI State")
        {
            var titleText = new TextField();
            titleText.value = nodeName;
            titleText.RegisterValueChangedCallback(evt =>
            {
                retNode.nodeName = evt.newValue;
            });
            retNode.titleContainer.Add(titleText);
        }

        retNode.titleContainer.Add(button);
        /*retNode.titleContainer.Add(new TextField
        {
            text = nodeName
        });*/
        retNode.mainContainer.Add(new Label("State Behaviour"));
        var condField = new ObjectField
        {
            objectType = typeof(ActorState)
        };
        condField.RegisterValueChangedCallback(evt => retNode.relevantState = evt.newValue as ActorState);
        retNode.mainContainer.Add(condField);
        retNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));
        retNode.RefreshExpandedState();
        retNode.RefreshPorts();

        return retNode;
    }

    public void AddConditionPort(ActorStateNode retNode, string overridenPortName = "", ActorStateTransitionCondition transCond = null)
    {
        var generatedPort = GeneratePort(retNode, Direction.Output);
        var outputPortCount = retNode.outputContainer.Query("connector").ToList().Count;

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);
        generatedPort.portName = (string.IsNullOrEmpty(overridenPortName) ? $"Condition {outputPortCount}" : overridenPortName);

        var condField = new ObjectField
        {
            objectType = typeof(ActorStateTransitionCondition),
            value = transCond
        };
        Label l = condField.Q<Label>();
        l.text = transCond == null ? "None" : transCond.name;
        condField.RegisterValueChangedCallback(evt => 
        {
            if(retNode.ports.Contains(evt.previousValue))retNode.ports.Remove(evt.previousValue as ActorStateTransitionCondition);
            ActorStateTransitionCondition cond = (evt.newValue as ActorStateTransitionCondition);
            l.text = cond.name;
            retNode.ports.Add(cond);
        });
        //condField.label = generatedPort.portName;
        //condField.RegisterValueChangedCallback(evt => generatedPort.contentContainer.GetFirstOfType<ObjectField>().value = evt.newValue);

        var deleteButton = new Button(() => RemovePort(retNode, generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(new Label("  "));
        generatedPort.contentContainer.Add(new IntegerField
        {
            label = "Priority: "
        });
        generatedPort.contentContainer.Add(condField);

        generatedPort.contentContainer.Add(deleteButton);

        retNode.outputContainer.Add(generatedPort);
        retNode.RefreshPorts();
        retNode.RefreshExpandedState();

    }

    private void RemovePort(ActorStateNode retNode, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);
        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }
        retNode.outputContainer.Remove(generatedPort);
        retNode.RefreshPorts();
        retNode.RefreshExpandedState();
    }

    public ActorStateNode GenerateEntryPointNode(ActorStateTransitionCondition cond = null)
    {
        var node = new ActorStateNode
        {
            title = "AWAKE",
            GUID = "0",
            relevantState = null,
            EntryPoint = true
        };

        var generatePort = GeneratePort(node, Direction.Output);
        generatePort.portName = "Next";

        var condField = new ObjectField
        {
            objectType = typeof(ActorStateTransitionCondition),
            value = (cond == null ? ScriptableObject.CreateInstance<AlwaysTrueTransitionCondition>() : cond)
        };

        generatePort.contentContainer.Add(condField);
        node.outputContainer.Add(generatePort);


        node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }
}
#endif
