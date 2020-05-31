using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
[Serializable]
public class ToolComponent : ItemComponent
{
    [SerializeField]
    public string anotherString;
/*
#if UNITY_EDITOR
    
    [CustomEditor(typeof(ItemComponent), true)]
    public class ToolComponentEditor : Editor
    {
        public ItemComponent targetObject;
        private void OnEnable()
        {
            targetObject = (ItemComponent)target;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            D
        }

    }
#endif
*/
}
