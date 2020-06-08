using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.TerrainAPI;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class PlacingUI : MonoBehaviour
{
    [SerializeField]
    PlayerController playerRef;
    [SerializeField][HideInInspector]
    public int gridSize;
    private CursorComponent activeUIComponent;
    public bool validAction;

    private SpriteRenderer sr;
    
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        validAction = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(activeUIComponent.snapToGrid)transform.position = new Vector3Int(Mathf.FloorToInt(mousePos.x),Mathf.FloorToInt(mousePos.y),0);
        else transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        if (Vector2.Distance(playerRef.gameObject.transform.position,transform.position) <= playerRef.interactRange)
        {
            sr.sprite = activeUIComponent.closeSprite;
            validAction = true;
        }
        else
        {
            sr.sprite = activeUIComponent.farSprite;
            validAction = false;
        }
    }

    public void updateUIComponent(CursorComponent uc)
    {
        activeUIComponent = uc;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PlacingUI), true)]
    public class PlacingUIEditor : Editor
    {
        PlacingUI ui;
        private void OnEnable()
        {
            ui = (PlacingUI)target;
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            ui.gridSize = EditorGUILayout.IntSlider("Tile Placer Size",ui.gridSize, 8, 200);
            serializedObject.ApplyModifiedProperties();
        }

    }

#endif
}
