using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;
    public event EventHandler<OnItemAddedToInventoryArgs> OnItemAddedToInventory;
    public class OnItemAddedToInventoryArgs : EventArgs
    {
        public ItemObject item;
    }
    public class OnInventoryActionArgs : EventArgs
    {
        public InputAction.CallbackContext context;
    }
    public event EventHandler OnRightClick;
    public event EventHandler<OnInventoryActionArgs> OnInventoryAction;
    private void Awake()
    {
        if(instance != null && instance!=this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public void fireAddItemToInventoryEvent(ItemObject item)
    {
        OnItemAddedToInventory?.Invoke(this,new OnItemAddedToInventoryArgs { item = item });
    }
    public void fireRightClickedEvent(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        OnRightClick?.Invoke(this, EventArgs.Empty);
    }
    public void fireInventoryActionEvent(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        OnInventoryAction?.Invoke(this, new OnInventoryActionArgs { context = context } );
    }
}
