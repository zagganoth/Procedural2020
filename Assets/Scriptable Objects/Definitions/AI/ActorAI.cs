using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Callbacks;
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
