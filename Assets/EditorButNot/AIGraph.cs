using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
//This code is largely from https://github.com/m3rt32/NodeBasedDialogueSystem
#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;


public class AIGraph : EditorWindow
{
    
    private ActorState _curState;
    private AIGraphView _graphView;
    private ActorAI destAI;
    private AIGraphSerializer _serializer;
    //[MenuItem("Graph/AI Graph")]
    public static void OpenDialogueGraphWindow(ActorAI AI)
    {
        AIGraph window = GetWindow<AIGraph>();
        window.titleContent = new GUIContent("AI Graph");
        window.destAI = AI;
        window._serializer = AIGraphSerializer.GetInstance(window._graphView);
        window._serializer.LoadGraph(AI);
    }
    private void OnEnable()
    {
        ConstructGraph();
        GenerateToolbar();
    }
    
    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        var stateBehaviour = new ObjectField("State: ");
        stateBehaviour.objectType = typeof(ActorState);
        stateBehaviour.SetValueWithoutNotify(_curState);
        stateBehaviour.MarkDirtyRepaint();
        stateBehaviour.RegisterValueChangedCallback(evt => _curState = evt.newValue as ActorState);
        toolbar.Add(stateBehaviour);

        var nodeCreateButton = new Button(() => { _graphView.CreateNode("New AI State");});
        nodeCreateButton.text = "Create State";
        toolbar.Add(nodeCreateButton);
        toolbar.Add(new Button(() => SaveOperation()) { text = "Save" });
        rootVisualElement.Add(toolbar);
    }

    private void SaveOperation()
    {
        if (destAI == null)
        {
            Debug.Log("No active object! Not saving");
            return;
        }
        if (_serializer == null)
            _serializer = AIGraphSerializer.GetInstance(_graphView);
        else
            _serializer.UpdateVars(_graphView);
        _serializer.SaveGraph(destAI);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }

    private void ConstructGraph()
    {
        _graphView = new AIGraphView
        {
            name = "AI Graph"
        };

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }
}
#endif