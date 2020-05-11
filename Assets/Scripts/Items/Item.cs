
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : MonoBehaviour
{

    [SerializeField]
    public ItemObject item;

    /*
    private void OnMouseDown()
    {
        EventManager.instance.fireAddItemToInventoryEvent(item);
        Destroy(gameObject);
    }*/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        Debug.Log(collision.gameObject.tag);
        if(collision.gameObject.CompareTag("Player"))
        {
            EventManager.instance.fireAddItemToInventoryEvent(item);
            Destroy(gameObject);
        }
    }
}
