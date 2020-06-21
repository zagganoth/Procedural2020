using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    [SerializeField]
    ActorAI behaviour;
    public Dictionary<string,object> tokens;
    private void Awake()
    {
        tokens = new Dictionary<string,object>();
    }
    void Start()
    {
        StartCoroutine(behaviour.Execute(this.gameObject));
    }


}
