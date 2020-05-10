using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragButton : MonoBehaviour, IBeginDragHandler, 
    IEndDragHandler, IDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    public int slotIndex;
    [SerializeField]
    BaseInventoryUIComponents parentInventory;
    private Image imageSprite;
    private bool isBeingDragged;
    private void Awake()
    {
        imageSprite = GetComponent<Image>();
        isBeingDragged = false;
        if(parentInventory == null)
        {
            parentInventory = GetComponentInParent<BaseInventoryUIComponents>();
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        isBeingDragged = true;
        parentInventory.slotClicked(slotIndex);
        imageSprite.color = new Color(0.75f, 0.75f, 0.75f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Do nothing
    }

    public void OnDrop(PointerEventData eventData)
    {
        parentInventory.slotClicked(slotIndex);
        imageSprite.color = new Color(1f, 1f, 1f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        imageSprite.color = new Color(1f,1f, 1f);
        parentInventory.slotClicked(slotIndex);
        isBeingDragged = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(DraggableObject.instance.dragging)
            imageSprite.color = new Color(0.75f, 1f, 0.75f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isBeingDragged)
            imageSprite.color = new Color(1f, 1f, 1f);
    }
}
