using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Slime Idle State", menuName = "Custom/AI/States/Slime Idle")]
public class SlimeIdleState : ActorState
{
    /*[SerializeField]
    float spotRadius;*/
    [SerializeField]
    float speed;

    Rigidbody2D rb;
    //bool playerIn
    protected override void InitializeVars()
    {
        rb = curObject.GetComponent<Rigidbody2D>();
    }

    protected override void ChildExecute()
    {
        float random = Random.Range(0f, 360f);
        rb.velocity = Vector2.zero;
        //curObject.transform.position += ;
        rb.AddForce(new Vector3(Mathf.Cos(random), Mathf.Sin(random)) * speed);

    }

    protected override void WrapUp()
    {
        rb.velocity = Vector2.zero;
    }
}
