
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : MonoBehaviour
{

    [SerializeField]
    public ItemObject item;
    [SerializeField]
    public bool pickable = true;
    SpriteRenderer sr;
    virtual protected void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
   virtual protected void Start()
    {
        if(sr != null && item != null && sr.sprite != item.image)
        {
            sr.sprite = item.image;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(pickable && collision.gameObject.CompareTag("Player"))
        {
            EventManager.instance.fireAddItemToInventoryEvent(item,collision.gameObject.GetComponent<PlayerController>());
            Destroy(gameObject);
        }
    }
}
