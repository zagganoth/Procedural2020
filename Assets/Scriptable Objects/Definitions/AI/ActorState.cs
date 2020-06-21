using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class ActorState : ScriptableObject
{
    public List<KeyValuePair<ActorStateTransitionCondition,ActorState>> conditions;
    protected GameObject curObject;
    public float updateFrequency = 0f;
    protected object curParams;
    public IEnumerator OnStateEnter(GameObject obj, object param)
    {
        curObject = obj;
        curParams = param;
        InitializeVars();
        yield return Execute();
    }
    protected abstract void InitializeVars();
    // Start is called before the first frame update
    protected virtual IEnumerator Execute()
    {
        bool breakTime = false;
        ActorState nextState = null;
        object nextParams = null;
        foreach(var condition in conditions)
        {
            condition.Key.Initialize(curObject);
        }
        while(true)
        {
            foreach(var condition in conditions)
            {
                if(condition.Key.Accept())
                {
                    nextState = condition.Value;
                    nextParams = condition.Key.stateParams;
                    breakTime = true;
                    break;
                }
            }
            if (breakTime)
                break;
            ChildExecute();
            yield return new WaitForSeconds(updateFrequency);
        }

        yield return OnStateExit(nextState,nextParams); 
    }

    protected abstract void ChildExecute();

    protected virtual IEnumerator OnStateExit(ActorState nextState, object param)
    {
        WrapUp();
        if(nextState!=null)
        {
            Debug.Log("Moving to state " + nextState);
            yield return nextState.OnStateEnter(curObject,param);
        }
        else
        {
            //Debug.Log("No next state, destroying gameObject!");
            Animator anim = curObject.GetComponent<Animator>() ;
            if(anim)
            {
                anim.SetTrigger("dead");
                yield return new WaitForSeconds(0.4f);
                Destroy(curObject);
            }

        }
        //yield return null;
    }
    protected abstract void WrapUp();
}
