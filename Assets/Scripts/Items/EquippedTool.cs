using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class EquippedTool : MonoBehaviour
{
    [SerializeField]
    PlayerInventoryUI inventoryUI;
    AnimatorController currentController;
    Animator anim;
    ItemObject item;
    SpriteRenderer sr;
    public LayerMask enemyLayers;
    private PlayerController playerRef;

    protected void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        playerRef = GetComponentInParent<PlayerController>();
    }
    protected  void Start()
    {
        gameObject.SetActive(false);
    }
    void Initialize()
    {
        gameObject.SetActive(true);
        SwingComponent sc;
        if((sc = item.getComponent("SwingComponent") as SwingComponent) && sc.animator != currentController)
        {
            currentController = sc.animator;
            anim.runtimeAnimatorController = currentController;
            sr.sprite = item.image;
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.activeSelf && collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
    }
    public void Use(direction dir)
    {
        gameObject.SetActive(true);
        item = inventoryUI.GetActiveItem();
        SwingComponent sc;
        if (item == null || !(sc = item.getComponent("SwingComponent") as SwingComponent)) return;
        //Type itemType = item.GetType();
        //if (itemType!=typeof(ToolObject) && !itemType.IsSubclassOf(typeof(ToolObject))) return;
        Initialize();
        anim.SetTrigger("use");
        switch(dir)
        {
            case direction.up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case direction.down:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case direction.left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case direction.right:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
        }

        StartCoroutine(endSwing(sc.swingTime));
    }
    private IEnumerator endSwing(float swingTime)
    {

        yield return new WaitForSeconds(swingTime);//1f);
        gameObject.SetActive(false);
    }
    private void Update()
    {
        transform.position = playerRef.transform.position;
    }
}
