using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.TerrainAPI;
using UnityEditor.Animations;
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
    Animator anim;
    public bool validAction;
    private AnimatorOverrideController aoc;
    private AnimatorController blank;
    private SpriteRenderer sr;
    private Sprite backupSprite;
    private bool updateSprite;
    
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        blank = (AnimatorController)anim.runtimeAnimatorController;
        updateSprite = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        validAction = false;
        anim.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!updateSprite) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(activeUIComponent.snapToGrid)transform.position = new Vector3Int(Mathf.FloorToInt(mousePos.x),Mathf.FloorToInt(mousePos.y),0);
        else transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        if (Vector2.Distance(Vector2Int.FloorToInt(playerRef.collider.transform.position),Vector2Int.FloorToInt(transform.position)) <= playerRef.interactRange)
        {
            sr.sprite = activeUIComponent.closeSprite;
            validAction = true;
        }
        else
        {
            //Debug.Log(Vector2Int.Distance(Vector2Int.RoundToInt(playerRef.rb.transform.position), Vector2Int.RoundToInt(transform.position)));
            sr.sprite = activeUIComponent.farSprite;
            validAction = false;
        }
    }

    public void updateUIComponent(CursorComponent uc)
    {
        activeUIComponent = uc;
        aoc = new AnimatorOverrideController(anim.runtimeAnimatorController);
        var l = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        if (activeUIComponent.anim)
        {
            l.Add(new KeyValuePair<AnimationClip, AnimationClip>(aoc.animationClips[0], activeUIComponent.anim));
        }
        else
        {
            l.Add(new KeyValuePair<AnimationClip, AnimationClip>(aoc.animationClips[0], blank.animationClips[0]));
        }
        aoc.ApplyOverrides(l);
        anim.runtimeAnimatorController = aoc;
    }

    public void playAnimation()
    {
        if (gameObject.activeSelf && activeUIComponent.anim)
        {
            backupSprite = sr.sprite;
            sr.sprite = null;
            anim.gameObject.SetActive(true);
            StartCoroutine(disableAnim(1f));
            updateSprite = false;
        }
    }
    private IEnumerator disableAnim(float time)
    {
        yield return new WaitForSeconds(time);
        sr.sprite = backupSprite;
        anim.gameObject.SetActive(false);
        updateSprite = true;
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
