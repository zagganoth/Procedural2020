using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class CursorComponent : ItemComponent
{
    public Sprite closeSprite;
    public Sprite farSprite;
    public bool snapToGrid = false;
    [SerializeField]
    public AnimationClip anim;
}
