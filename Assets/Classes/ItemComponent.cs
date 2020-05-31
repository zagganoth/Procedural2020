using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class ItemComponent : ScriptableObject
{

    [SerializeField]
    public string tag;
    private void OnEnable()
    {
        tag = this.GetType().ToString();
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ItemComponent), true)]
    public class ItemComponentEditor : Editor
    {
        public ItemComponent targetObject;
        private void OnEnable()
        {
            targetObject = (ItemComponent)target;
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }

    }
#endif
}
