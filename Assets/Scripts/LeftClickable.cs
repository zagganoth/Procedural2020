using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.Events;

public class LeftClickable : MonoBehaviour
{
    private void Start()
    {
        EventManager.instance.OnSuccessfulLeftClick += invokeEvent;
    }
    [Serializable]
    public class LeftClickUEvent : UnityEvent<EventManager.OnLeftClickArgs> { };
    [SerializeField]
    public LeftClickUEvent action;
    public void invokeEvent(object sender, EventManager.OnLeftClickArgs args)
    {
        if (args.instanceId == transform.gameObject.GetInstanceID())
        {
            action.Invoke(args);
        }
    }
}
