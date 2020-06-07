using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using Unity.Entities.UniversalDelegates;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum tileSpritePositions
{
    tLeft = 0,
    top = 1,
    tRight = 2,
    left,
    def,
    right,
    bLeft,
    bottom,
    bRight
}
[Serializable][CreateAssetMenu(fileName ="New Wall Tile",menuName = "2D Extras/Tiles/Wall Tile")]
public class WallRuleTile : TileBase
{
    RuleTile sdaf;
    [SerializeField]
    public List<Sprite> possibleSprites;
    [SerializeField]
    public Sprite relevantTile;

    private void OnEnable()
    {
        //possibleSprites = new List<Sprite>();
        CreateList();
    }
    private void ResetSprites()
    {
        possibleSprites.Clear();
        CreateList();
    }
    private void CreateList()
    {
        for (int i = possibleSprites.Count; i < 9; i++)
        {
            possibleSprites.Add(null);
        }
    }
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        var iden = Matrix4x4.identity;
        tileData.sprite = possibleSprites[(int)tileSpritePositions.def];
        tileData.gameObject = null;
        tileData.flags = TileFlags.LockTransform;
        tileData.transform = iden;
        tileData.colliderType = Tile.ColliderType.Sprite;
        Vector3Int up = position + new Vector3Int(0, 1, 0);
        Vector3Int down = position + new Vector3Int(0, -1, 0);
        Vector3Int left = position + new Vector3Int(-1, 0, 0);
        Vector3Int right = position + new Vector3Int(1, 0, 0);
        Vector3Int topLeft = position + new Vector3Int(-1, 1, 0);
        Vector3Int topRight = position + new Vector3Int(1, 1, 0);
        Vector3Int bottomLeft = position + new Vector3Int(-1, -1, 0);
        Vector3Int bottomRight = position + new Vector3Int(1, -1, 0);
        Sprite upSprite = tilemap.GetSprite(up);
        Sprite downSprite = tilemap.GetSprite(down);
        Sprite leftSprite = tilemap.GetSprite(left);
        Sprite rightSprite = tilemap.GetSprite(right);
        Sprite bottomLeftSprite = tilemap.GetSprite(bottomLeft);
        Sprite bottomRightSprite = tilemap.GetSprite(bottomRight);
        Sprite topLeftSprite = tilemap.GetSprite(topLeft);
        Sprite topRightSprite = tilemap.GetSprite(topRight);

        if (!rightSprite && !upSprite && leftSprite && downSprite)
        {
            tileData.sprite = possibleSprites[(int)tileSpritePositions.tRight];
        }
        else if (rightSprite && downSprite && !upSprite && !leftSprite)
        {
            tileData.sprite = possibleSprites[(int)tileSpritePositions.tLeft];
        }
        else if (rightSprite && upSprite && !downSprite && !leftSprite)//right == possibleSprites[(int)tileSpritePositions.bottom] || right == possibleSprites[(int)tileSpritePositions.def]) && (up == possibleSprites[(int)tileSpritePositions.left] || up == possibleSprites[(int)tileSpritePositions.def]))
        {
            tileData.sprite = possibleSprites[(int)tileSpritePositions.bLeft];
        }
        else if (leftSprite && upSprite && !downSprite && !rightSprite)//(left == possibleSprites[(int)tileSpritePositions.bottom] || left == possibleSprites[(int)tileSpritePositions.def]) && (up == possibleSprites[(int)tileSpritePositions.right] || up == possibleSprites[(int)tileSpritePositions.def]))
        {
            tileData.sprite = possibleSprites[(int)tileSpritePositions.bRight];
        }
        
        else if ( !leftSprite && !rightSprite)
        {
            if (
                (upSprite == possibleSprites[(int)tileSpritePositions.right])
                || (downSprite == possibleSprites[(int)tileSpritePositions.right])
                || (upSprite == possibleSprites[(int)tileSpritePositions.tRight] && topLeftSprite)
                || (downSprite == possibleSprites[(int)tileSpritePositions.bRight] && bottomLeftSprite)
            )
            {
                tileData.sprite = possibleSprites[(int)tileSpritePositions.right];
            }
            else if(upSprite || downSprite)
            {
                tileData.sprite = possibleSprites[(int)tileSpritePositions.left];
            }
        }
        else
        {
            if(
                (leftSprite == possibleSprites[(int)tileSpritePositions.top])
                || (rightSprite == possibleSprites[(int)tileSpritePositions.top])
                || (leftSprite == possibleSprites[(int)tileSpritePositions.tLeft] && bottomLeftSprite)
                || (rightSprite == possibleSprites[(int)tileSpritePositions.tRight] && bottomRightSprite)
            )
            {
                tileData.sprite = possibleSprites[(int)tileSpritePositions.top];
            }
            else if(leftSprite||rightSprite)
            {
                tileData.sprite = possibleSprites[(int)tileSpritePositions.bottom];
            }
        }
        //base.GetTileData(position, tilemap, ref tileData);

    }
#if UNITY_EDITOR
    [CustomEditor(typeof(WallRuleTile), true)]
    public class WallRuleTileEditor : Editor
    {
        WallRuleTile targetObject;
        public float tileSize;
        private void OnEnable()
        {
            tileSize = 71;
            targetObject = (WallRuleTile)target;

        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            tileSize = EditorGUILayout.Slider(tileSize, 0, 128);

            //if (targetObject.relevantTile == null) return;
            DrawBoxes(3);
            DrawBoxes(3, 3);
            DrawBoxes(3,6);
            if(GUILayout.Button("Reset"))
            {
                targetObject.ResetSprites();
            }
            serializedObject.ApplyModifiedProperties();
        }
        public void DrawBoxes(int count,int offset=0,int spaceAtIndex=int.MaxValue)
        {

            EditorGUILayout.BeginHorizontal();
            GUIStyle centeredImageStyle = GUI.skin.box;
            centeredImageStyle.alignment = TextAnchor.MiddleCenter;
            centeredImageStyle.normal.textColor = GUI.skin.button.normal.textColor;
            centeredImageStyle.border = new RectOffset(10, 1, 1, 1);
            centeredImageStyle.fontSize = 11;
            GUIStyle centeredStyle = GUI.skin.box;
            centeredStyle.alignment = centeredImageStyle.alignment;
            centeredStyle.normal.textColor = centeredStyle.normal.textColor;
            centeredStyle.border = new RectOffset(10, 1, 1, 1);
            Color oldColor = GUI.color;
            GUI.backgroundColor = new Color(0, 0, 0);
            //if(targetObject.relevantTile != null)
            Rect curRect;
            for (int i = 0; i < count;i++)
            {
                if (i==spaceAtIndex)
                {
                    GUILayout.Space(tileSize + 15);
                }
                curRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(tileSize), GUILayout.Width(tileSize));
                if (i + offset < targetObject.possibleSprites.Count && targetObject.possibleSprites[i + offset] != null)
                {
                    GUI.backgroundColor = new Color(255,255,255,0);
                    Sprite sprite = targetObject.possibleSprites[i + offset];
                    var croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                    var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                            (int)sprite.textureRect.y,
                                                            (int)sprite.textureRect.width,
                                                            (int)sprite.textureRect.height);
                    croppedTexture.SetPixels(pixels);
                    croppedTexture.Apply();
                    GUI.DrawTexture(curRect, croppedTexture,ScaleMode.StretchToFill);
                    GUI.Label(curRect, "["+Enum.GetName(typeof(tileSpritePositions), i + offset)+"]",centeredImageStyle);
                    GUI.backgroundColor = new Color(0, 0, 0);
                }
                else
                    GUI.Box(curRect, "[" + Enum.GetName(typeof(tileSpritePositions), i + offset) + "]", centeredStyle);
                DraggingAndDropping(curRect,i+offset);

                GUILayout.Space(15);
            }
            GUI.backgroundColor = oldColor;

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(15);

        }
        private void DraggingAndDropping(Rect dropArea,int index)
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
                        Sprite spr = DragAndDrop.objectReferences[i] as Sprite;
                        //Type componentType = script.GetClass();
                        //Sprite inst = (Sprite)componentType;
                        targetObject.possibleSprites[index] = spr;
                        //subEditors.Add(CreateEditor(targetObject.components[targetObject.components.Count - 1]) as ItemComponent.ItemComponentEditor);
                        //showingComponent.Add(false);
                        EditorUtility.SetDirty(this);
                        EditorUtility.SetDirty(targetObject);
                        //EditorUtility.SetDirty();
                        /*string _path = AssetDatabase.GetAssetPath(target);
                        AssetDatabase.AddObjectToAsset(targetObject.components[targetObject.components.Count - 1], _path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();*/

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
                if (DragAndDrop.objectReferences[i].GetType() != typeof(Sprite))
                    return false;
            }

            // If none of the dragging objects returned that the drag was invalid, return that it is valid.
            return true;
        }
        public static Texture2D textureFromSprite(Sprite sprite)
        {
            if (sprite.rect.width != sprite.texture.width)
            {
                Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                             (int)sprite.textureRect.y,
                                                             (int)sprite.textureRect.width,
                                                             (int)sprite.textureRect.height);
                newText.SetPixels(newColors);
                newText.Apply();
                return newText;
            }
            else
                return sprite.texture;
        }
    }
#endif
}

