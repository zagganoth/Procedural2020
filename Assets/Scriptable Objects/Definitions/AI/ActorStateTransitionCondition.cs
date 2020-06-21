using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
[CreateAssetMenu(fileName = "False Condition", menuName = "Custom/AI/Transition Conditions/False")]
public class ActorStateTransitionCondition : ScriptableObject
{
    
    public object stateParams = null;
    public virtual void Initialize(GameObject actor)
    {

    }
    public virtual bool Accept()
    {
        return false;
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ActorStateTransitionCondition), true)]
    public class ActorStateTransitionEditor : Editor
    {
        bool expanded;
        HashSet<string> badWords;
        GUILayoutOption[] options;
        bool hasProperties;
        //SerializedObject _target;
        private void OnEnable()
        {
            badWords = new HashSet<string>();
            badWords.Add("m_Script");
            badWords.Add("Base");
            options = new GUILayoutOption[]{ GUILayout.MaxWidth(100f),GUILayout.ExpandWidth(false),GUILayout.Width(100f)};
            hasProperties = false;
            serializedObject.Update();
            var p = serializedObject.GetIterator();
            do
            {
                if(!badWords.Contains(p.name))
                {
                    Debug.Log(p.name);
                    hasProperties = true;
                    break;
                }
                //if (p.name == "someProperty")
                //{
                //    // Add extra GUI after "someProperty"
                //}
            }
            while (p.NextVisible(true));
            //_target = (SerializedObject)target;
        }

        public override void OnInspectorGUI()
        {
            //GUILayout.ExpandWidth(false);
            //GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(100.0f), GUILayout.MinWidth(10.0f) };
            if (!hasProperties) return;
            expanded = EditorGUILayout.Foldout(expanded, "Fields",true);

            if (expanded)
            {
                EditorGUIUtility.labelWidth = 70f;
                //DrawPropertiesExcluding(serializedObject, "Script");
                serializedObject.Update();
                var p = serializedObject.GetIterator();
                do
                {
                    if (!badWords.Contains(p.name))
                        EditorGUILayout.PropertyField(p,options);
                    //if (p.name == "someProperty")
                    //{
                    //    // Add extra GUI after "someProperty"
                    //}
                }
                while (p.NextVisible(true));
            }
        }
        public void Val()
        {
            DrawDefaultInspector();
        }
    }
#endif
}