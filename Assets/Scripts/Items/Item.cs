
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : MonoBehaviour
{

    [SerializeField]
    public ItemObject item;
    SpriteRenderer sr;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        if(sr != null && item != null && sr.sprite != item.image)
        {
            sr.sprite = item.image;
        }
    }
    /*
    private void OnMouseDown()
    {
        EventManager.instance.fireAddItemToInventoryEvent(item);
        Destroy(gameObject);
    }*/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            EventManager.instance.fireAddItemToInventoryEvent(item,collision.gameObject.GetComponent<PlayerController>());
            Destroy(gameObject);
        }
    }
}
