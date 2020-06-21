using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Slime Angry State",menuName ="Custom/AI/States/Slime Angry")]
public class SlimeAngryState : ActorState
{
    private GameObject playerRef;
    [SerializeField]
    private float speed;
    protected override void InitializeVars()
    {
        playerRef = curParams as GameObject;
    } 

    protected override void ChildExecute()
    {
        curObject.transform.position = Vector2.MoveTowards(curObject.transform.position, playerRef.transform.position, speed);
    }

    protected override void WrapUp()
    {
        
    }
}
