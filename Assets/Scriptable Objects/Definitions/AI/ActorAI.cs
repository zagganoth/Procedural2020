using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Callbacks;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;

public class AssetHandler
{
    [OnOpenAsset()]
    public static bool OpenEditor(int instanceId, int line)
    {
        ActorAI obj = EditorUtility.InstanceIDToObject(instanceId) as ActorAI;
        if(obj != null)
        {
            AIGraph.OpenDialogueGraphWindow(obj);
            return true;
        }
        return false;
    }
}

#endif

[Serializable][CreateAssetMenu(fileName ="New Actor AI", menuName = "Custom/AI/Actor AI")]
public class ActorAI : ScriptableObject
{
    public AIGraphContainer editorGraphContainer;
    public ActorState head;
    public List<ActorState> states;
    public IEnumerator Execute(GameObject attachedObject)
    {
        states = new List<ActorState>();
        if (editorGraphContainer == null) {
            Debug.LogError("Attempting to execute AI without any saved data!");
            yield return null;
        }
        int index = 0;
        List<KeyValuePair<ActorStateTransitionCondition, ActorState>> conds;
        int entryPointIndex = 0;
        foreach(var node in editorGraphContainer.nodeData)
        {
            if (node.relevantState == null) continue;
            conds = new List<KeyValuePair<ActorStateTransitionCondition, ActorState>>();
            states.Add(node.relevantState);
            foreach (var port in node.ports)
            {
                conds.Add(new KeyValuePair<ActorStateTransitionCondition, ActorState>(port.cond, port.destState));
            }
            states[index].conditions = conds;
            if (node.title == "Idle") entryPointIndex = index;
            //states[index].conditions = new Keynode.ports;
            index++;
        }
        //states[0].updateFrequency = 0.5f;
        if (states.Count > 0)
        {
            yield return states[entryPointIndex].OnStateEnter(attachedObject, null);
        }
        yield return null;
        /*
        if (states.Any(x => x.GUID == "0"))
        {
            head = editorGraphContainer.nodeData.Find(x => x.GUID == "0");
            
            /*foreach (var condPort in head.ports)
            {
                if(condPort.cond.Accept(attachedObject))condPort.
            }
        }*/
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ActorAI), true)]
    public class ActorAIEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor"))
            {
                AIGraph.OpenDialogueGraphWindow(target as ActorAI);
            }
        }
    }
#endif
} 
