using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Tilemaps;
public enum direction
{
    up,
    down,
    left,
    right
}
public class PlayerController : MonoBehaviour
{
    Vector2 inputVector;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    EquippedTool equippedTool;
    Animator swingWeaponAnimator;
    ItemObject equippedItemObject;
    SpriteRenderer equippedItemSprite;
    SpriteRenderer characterSprite;
    [SerializeField]
    Sprite[] directionalSprites;

    [SerializeField]
    public int inventoryIndex;
    private bool swinging;
    [SerializeField]
    PlayerInventoryUI inventoryUI;
    [SerializeField]
    Tilemap structureOverMap;
    private bool structureDisabled;
    private Rigidbody2D rb;
    private bool colliding;
    [SerializeField]
    public float interactRange;

    direction currentDirection;
    Dictionary<direction, string> directionBools;
    Dictionary<direction, Vector3> directionVectors;
    public BoxCollider2D collider;
    private void Awake()
    {
        //moveSpeed *= 50;
        initializeDirectionBools();
        currentDirection = direction.down;
        characterSprite = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        //equippedItemObject = equippedItem.item;
        //swingWeaponAnimator = equippedItem.gameObject.GetComponent<Animator>();
        //equippedItemSprite = equippedItem.gameObject.GetComponent<SpriteRenderer>();
        //EventManager.instance.OnItemAddedToInventory += changeEquippedItem;
        swinging = false;
        structureDisabled = false;
        rb = GetComponent<Rigidbody2D>();
    }
    /*private void changeEquippedItem(object sender, EventManager.OnItemAddedToInventoryArgs e)
    {
        //equippedItemObject = e.item;
        //equippedItem.item = e.item;
    }*/
    private void initializeDirectionBools()
    {
        directionBools = new Dictionary<direction, string>();
        directionBools[direction.up] = "swingUp";
        directionBools[direction.down] = "swingDown";
        directionBools[direction.left] = "swingLeft";
        directionBools[direction.right] = "swingRight";
        directionVectors = new Dictionary<direction, Vector3>();
        directionVectors[direction.up] = new Vector3(0,+1,0);
        directionVectors[direction.down] = new Vector3(0,-1,0);
        directionVectors[direction.left] = new Vector3(-1,0,0);
        directionVectors[direction.right] = new Vector3(+1,0,0);
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        //if (context.performed) return;
        Vector2 input = context.ReadValue<Vector2>();
        inputVector = input * moveSpeed;
        if (input.y > 0)
        {
            currentDirection = direction.up;
        }
        else if (input.x > 0)
        {
            currentDirection = direction.right;
        }
        else if (input.y < 0)
        {
            currentDirection = direction.down;
        }
        else if(input.x < 0)
        {
            currentDirection = direction.left;
        }

        characterSprite.sprite = directionalSprites[(int)currentDirection];
        rb.velocity = inputVector;
    }
    /*
    public ItemObject getEquippedItem()
    {
        ItemObject activeItem = inventoryUI.GetActiveItem();
        if(activeItem != null && activeItem.image != equippedItemSprite.sprite)
        {
            equippedItemSprite.sprite = activeItem.image;
        }
        return activeItem;
    }*/
    public void OnSwing(InputAction.CallbackContext context)
    {
        if (!context.performed || swinging)
        {
            swinging = true;
            equippedTool.Use(currentDirection);
        }
        StartCoroutine(endSwing());
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        colliding = true;
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        rb.velocity = inputVector;
    }
    IEnumerator endSwing()
    {
        yield return new WaitForSeconds(0.2f);
        //swingWeaponAnimator.gameObject.SetActive(false);
        swinging = false;
    }
    private void Update()
    {
        //rb.velocity = inputVector * Time.deltaTime;
        //transform.position += ;
        //if(!colliding)rb.velocity = new Vector3(inputVector.x, inputVector.y, 0) * Time.deltaTime;
        if (structureOverMap.GetTile(Vector3Int.FloorToInt(transform.position)) != null)
        {
            structureOverMap.gameObject.SetActive(false);
            structureDisabled = true;
        }
        else if(structureDisabled)
        {
            structureDisabled = false;
            structureOverMap.gameObject.SetActive(true);

        }
    }
}
