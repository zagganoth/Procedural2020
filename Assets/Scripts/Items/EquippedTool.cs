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
    ToolObject tool;
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
    }
    void Initialize()
    {
        gameObject.SetActive(true);
        tool = item as ToolObject;
        if (currentController != tool.animator)
        {
            currentController = tool.animator;
            anim.runtimeAnimatorController = currentController;
            sr.sprite = item.image;
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
    }
    public void Use(direction dir)
    {

        item = inventoryUI.GetActiveItem();
        Type itemType = item.GetType();
        if (itemType!=typeof(ToolObject) && !itemType.IsSubclassOf(typeof(ToolObject))) return;
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

        StartCoroutine(endSwing());
    }
    private IEnumerator endSwing()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
    private void Update()
    {
        transform.position = playerRef.transform.position;
    }
}
