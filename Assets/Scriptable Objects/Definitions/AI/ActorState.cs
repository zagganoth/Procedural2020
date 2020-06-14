using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class ActorState : MonoBehaviour
{
    protected virtual void OnStateEnter()
    {

    }
    // Start is called before the first frame update
    protected virtual IEnumerator Execute()
    {
        yield return null;
        OnStateExit(null);
    }

    protected virtual void OnStateExit(ActorState nextState)
    {
        if(nextState!=null)
        {
            nextState.OnStateEnter();
        }
        Destroy(this);
    }
}
