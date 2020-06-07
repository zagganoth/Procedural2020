using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;
    public event EventHandler<OnItemAddedToInventoryArgs> OnItemAddedToInventory;
    //private HashSet<ItemObject> subscripInitialized;
    public class OnItemAddedToInventoryArgs : EventArgs
    {
        public ItemObject item;
        public PlayerController playerRef;
    }
    public class OnInventoryActionArgs : EventArgs
    {
        public InputAction.CallbackContext context;
    }
    public class OnRightClickArgs : EventArgs
    {
        public InputAction.CallbackContext context;
        public Vector3 clickPosition;
    }
    public class OnCleanupRequiredArgs : EventArgs
    {
        public Vector3Int pos;
    }
    public event EventHandler<OnRightClickArgs> OnRightClick;
    public event EventHandler<OnInventoryActionArgs> OnInventoryAction;

    public event EventHandler<OnCleanupRequiredArgs> OnCleanupRequiredAction;
    private void Awake()
    {
        if(instance != null && instance!=this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        //subscripInitialized = new HashSet<ItemObject>();
    }
    public void fireAddItemToInventoryEvent(ItemObject item, PlayerController player)
    {
        /*if (!subscripInitialized.Contains(item))
        {
            item.subscribeToEvents();
            subscripInitialized.Add(item);
        }*/
        OnItemAddedToInventory?.Invoke(this,new OnItemAddedToInventoryArgs { item = item, playerRef = player });
    }
    public void fireRightClickedEvent(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        OnRightClick?.Invoke(this, new OnRightClickArgs { context = context, clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) });  ;
    }
    public void fireInventoryActionEvent(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        OnInventoryAction?.Invoke(this, new OnInventoryActionArgs { context = context } );
    }
    
    public void fireCleanupTileAction(Vector3 position)
    {
        OnCleanupRequiredAction?.Invoke(this, new OnCleanupRequiredArgs { pos = Vector3Int.FloorToInt(position) });
    }
}
