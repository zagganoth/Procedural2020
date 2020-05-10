using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector2 inputVector;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    Item equippedItem;
    Animator swingWeaponAnimator;
    ItemObject equippedItemObject;

    SpriteRenderer characterSprite;
    [SerializeField]
    Sprite[] directionalSprites;
    public LayerMask enemyLayers;
    [SerializeField]
    public int inventoryIndex;

    private bool swinging;
    enum direction
    {
        up,
        down,
        left,
        right
    }
    direction currentDirection;
    Dictionary<direction, string> directionBools;
    Dictionary<direction, Vector3> directionVectors;
    private void Awake()
    {
        moveSpeed /= 100;
        initializeDirectionBools();
        currentDirection = direction.down;
        characterSprite = GetComponent<SpriteRenderer>();
        equippedItemObject = equippedItem.item;
        swingWeaponAnimator = equippedItem.gameObject.GetComponent<Animator>();
        swinging = false;
    }
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
    }
    public ItemObject getEquippedItem()
    {
        ItemObject equippedItem =  InventoryManager.instance.activeInventories[inventoryIndex].items[UIManager.instance.activeSlotIndex];
        return equippedItem;
    }
    public void OnSwing(InputAction.CallbackContext context)
    {

        if (!context.performed || swinging || !getEquippedItem().canSwing()) return;
        swingWeaponAnimator.gameObject.SetActive(true);
        swingWeaponAnimator.SetTrigger(directionBools[currentDirection]);
        swinging = true;
        StartCoroutine(endSwing(currentDirection));
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position + directionVectors[currentDirection], 0.5f,enemyLayers);
        foreach(Collider2D enemy in collisions)
        {
            Destroy(enemy.gameObject);
        }

    }
    
    IEnumerator endSwing(direction dir)
    {
        yield return new WaitForSeconds(0.2f);
        swingWeaponAnimator.gameObject.SetActive(false);
        swinging = false;
    }
    private void Update()
    {
        transform.position += new Vector3(inputVector.x, inputVector.y, 0);

    }
}
