using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RockAngerBehavior : StateMachineBehaviour
{
    GameObject ProjectilePrefab;
    [SerializeField]
    int numProjectiles;
    [SerializeField]
    float baseProjectileDistance;
    List<GameObject> projectiles;
    float smallestAngle;
    int counter;
    RockData rockDataRef;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rockDataRef = animator.gameObject.GetComponent<RockData>();
        ProjectilePrefab = rockDataRef.projectilePrefab;
        projectiles = new List<GameObject>();
        //Vector3 curPos = new Vector3(0,baseProjectileDistance);
        Vector3 addition;
        float x;
        float angle;
        //opp/hyp = invSin, hyp = baseProjectileDistance, therefore opp = hyp * invSin
        for (int i = 0; i < numProjectiles; i++)
        {
            //thanks, https://answers.unity.com/questions/1068513/place-8-objects-around-a-target-gameobject.html
            angle = i * Mathf.PI * 2f / numProjectiles;
            addition = new Vector3(Mathf.Cos(angle) * baseProjectileDistance, Mathf.Sin(angle) * baseProjectileDistance);

            projectiles.Add(Instantiate(ProjectilePrefab, animator.gameObject.transform.position + addition, Quaternion.identity));
        }
        smallestAngle = 2f * Mathf.PI / numProjectiles;
        rockDataRef.call_arc(projectiles, smallestAngle, baseProjectileDistance, animator);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        //StartCoroutine(moveInArc())
    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
