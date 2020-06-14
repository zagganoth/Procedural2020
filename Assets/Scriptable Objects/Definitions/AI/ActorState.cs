using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class ActorState : MonoBehaviour
{
    public List<KeyValuePair<ActorStateTransitionCondition,ActorState>> conditions;
    public GameObject curObject;
    public float updateFrequency = 0f;
    protected virtual void OnStateEnter(GameObject obj)
    {
        curObject = obj;
    }
    // Start is called before the first frame update
    protected virtual IEnumerator Execute()
    {
        bool breakTime = false;
        ActorState nextState = null;
        while(true)
        {
            foreach(var condition in conditions)
            {
                if(condition.Key.Accept(curObject))
                {
                    nextState = condition.Value;
                    breakTime = true;
                    break;
                }
            }
            if (breakTime)
                break;
            yield return new WaitForSeconds(updateFrequency);
        }

        OnStateExit(nextState); 
    }

    protected virtual void OnStateExit(ActorState nextState)
    {
        if(nextState!=null)
        {
            nextState.OnStateEnter(curObject);
        }
        Destroy(this);
    }
}
