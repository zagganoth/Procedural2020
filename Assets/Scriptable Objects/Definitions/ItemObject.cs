using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable][CreateAssetMenu(fileName = "New Item", menuName = "Custom/Inventory System/Item")]
public class ItemObject : ScriptableObject
{
    public Sprite image;
    public int maxStackSize;
    //public int count;
    [HideInInspector]
    [SerializeField]
    private List<ItemComponent> components;
    private Dictionary<string, ItemComponent> componentsDict;
    
    public virtual void subscribeToEvents()
    {

    }
    private void OnEnable()
    {
        componentsDict = new Dictionary<string, ItemComponent>();
        foreach (var comp in components)
        {
            componentsDict.Add(comp.tag, comp);
        }
    }
    private void Awake()
    {
        //maxStackSize = 1;
    }
    public ItemComponent getComponent(string tag)
    {
        ItemComponent retVal = new ItemComponent();
        componentsDict.TryGetValue(tag, out retVal);
        return retVal;
    }
    public bool hasComponent(string tag)
    {
        return componentsDict.ContainsKey(tag);
    }
    public virtual void use(Vector3 location, Tilemap tim, PlayerController playerRef)
    {

    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ItemObject), true)]
    public class ItemEditor : Editor
    {
        ItemObject targetObject;
        SerializedObject so;
        public List<bool> showingComponent;
        public List<ItemComponent.ItemComponentEditor> subEditors;
        bool showingDict;
        private void OnEnable()
        {
            targetObject = (ItemObject)target;
            so = new SerializedObject(targetObject);
            subEditors = new List<ItemComponent.ItemComponentEditor>();
            showingComponent = new List<bool>();
            if (targetObject == null || targetObject.components == null) return;
            foreach (ItemComponent v in targetObject.components)
            {
                subEditors.Add(CreateEditor(v) as ItemComponent.ItemComponentEditor);
                showingComponent.Add(false);
            }

        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            //DrawDictionary();
            EditorGUILayout.LabelField("Components: " + targetObject.components.Count);

            EditorGUI.indentLevel++;
            //EditorGUILayout.LabelField("subEditors: " + subEditors.Count);
            for (int i =0; i < targetObject.components.Count;i++)
            {
                //EditorGUILayout.ObjectField(targetObject.components[i], typeof(ItemComponent), false);


                if (subEditors[i] != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    showingComponent[i] = EditorGUILayout.Foldout(showingComponent[i], targetObject.components[i].GetType().ToString());
                    if (GUILayout.Button("-"))
                    {
                        AssetDatabase.RemoveObjectFromAsset(targetObject.components[i]);
                        subEditors.RemoveAt(i);
                        targetObject.components.RemoveAt(i);
                        showingComponent.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                    if (showingComponent[i])
                    {
                        subEditors[i].OnInspectorGUI();


                    }
                }
            }
            
            if (targetObject.components.Count == 0)
            {
                AddNewComponent();
            }
            else
            {
                AddNewComponent();
                if (GUILayout.Button("Clear"))
                {

                    for(int i = 0; i < targetObject.components.Count;i++)
                    {
                        AssetDatabase.RemoveObjectFromAsset(targetObject.components[i]);
                    }
                    targetObject.components.Clear();
                    subEditors.Clear();

                }
                EditorGUI.indentLevel--;
            }
            DrawDictionary();
            serializedObject.ApplyModifiedProperties();
        }
        public void DrawDictionary()
        {
            showingDict = EditorGUILayout.Foldout(showingDict, "Show Components Dictionary");
            if(showingDict)
            {
                foreach(var elem in targetObject.componentsDict)
                {
                    
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    EditorGUILayout.LabelField("Key: ");
                    EditorGUILayout.TextArea(elem.Key);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    EditorGUILayout.LabelField("Value: ");
                    EditorGUILayout.ObjectField(elem.Value, typeof(ItemComponent), false);
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        //Several of the below functions are more-or-less straight from https://learn.unity.com/tutorial/adventure-game-phase-4-reactions?projectId=5c514af7edbc2a001fd5c012#5c7f8528edbc2a002053b390
        public void AddNewComponent()
        {
            // Create a GUI style of a box but with middle aligned text and button text color.
            Rect fullWidthRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(50 + EditorGUIUtility.standardVerticalSpacing));
            GUIStyle centredStyle = GUI.skin.box;
            centredStyle.alignment = TextAnchor.MiddleCenter;
            centredStyle.normal.textColor = GUI.skin.button.normal.textColor;
            centredStyle.border = new RectOffset(10,1,1,1);
            // Draw a box over the area with the created style.
            Color oldColor = GUI.color;
            GUI.backgroundColor = new Color(0, 0, 0);

            GUI.Box(fullWidthRect, "Drop new components here", centredStyle);
            GUI.backgroundColor = oldColor;
            DraggingAndDropping(fullWidthRect);
        }
        private void DraggingAndDropping(Rect dropArea)
        {
            // Cache the current event.
            Event currentEvent = Event.current;

            // If the drop area doesn't contain the mouse then return.
            if (!dropArea.Contains(currentEvent.mousePosition))
                return;
            switch (currentEvent.type)
            {
                // If the mouse is dragging something...
                case EventType.DragUpdated:
                    DragAndDrop.visualMode = IsDragValid() ? DragAndDropVisualMode.Link : DragAndDropVisualMode.Rejected;
                    currentEvent.Use();
                    break;
                // If the mouse was dragging something and has released...
                case EventType.DragPerform:
                    DragAndDrop.AcceptDrag();
                    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    {
                        MonoScript script = DragAndDrop.objectReferences[i] as MonoScript;
                        Type componentType = script.GetClass();
                        ItemComponent inst = (ItemComponent)CreateInstance(componentType);
                        targetObject.components.Add(inst);
                        subEditors.Add(CreateEditor(targetObject.components[targetObject.components.Count - 1]) as ItemComponent.ItemComponentEditor);
                        showingComponent.Add(false);
                        EditorUtility.SetDirty(this);
                        EditorUtility.SetDirty(inst);
                        string _path = AssetDatabase.GetAssetPath(target);
                        AssetDatabase.AddObjectToAsset(targetObject.components[targetObject.components.Count-1], _path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                    }

                    // Make sure the event isn't used by anything else.
                    currentEvent.Use();

                    break;
            }
        }
        private bool IsDragValid()
        {
            // Go through all the objects being dragged...
            for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
            {
                // ... and if any of them are not script assets, return that the drag is invalid.
                if (DragAndDrop.objectReferences[i].GetType() != typeof(MonoScript))
                    return false;

                // Otherwise find the class contained in the script asset.
                MonoScript script = DragAndDrop.objectReferences[i] as MonoScript;
                Type scriptType = script.GetClass();
                foreach(var comp in targetObject.components)
                {
                    if (comp.GetType() == scriptType) return false;
                }
                // If the script does not inherit from Reaction, return that the drag is invalid.
                if (!scriptType.IsSubclassOf(typeof(ItemComponent)) && scriptType != typeof(ItemComponent))
                    return false;

                // If the script is an abstract, return that the drag is invalid.
                if (scriptType.IsAbstract)
                    return false;
            }

            // If none of the dragging objects returned that the drag was invalid, return that it is valid.
            return true;
        }
    }
#endif

}
