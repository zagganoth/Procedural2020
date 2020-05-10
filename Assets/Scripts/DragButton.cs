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
    LayoutGroup parentInventory;
    private Image imageSprite;
    private bool isBeingDragged;
    private void Awake()
    {
        imageSprite = GetComponent<Image>();
        isBeingDragged = false;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        isBeingDragged = true;
        UIManager.instance.ItemClicked(slotIndex,parentInventory);
        imageSprite.color = new Color(0.75f, 0.75f, 0.75f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Do nothing
    }

    public void OnDrop(PointerEventData eventData)
    {
        UIManager.instance.ItemClicked(slotIndex,parentInventory);
        imageSprite.color = new Color(1f, 1f, 1f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        imageSprite.color = new Color(1f,1f, 1f);
        UIManager.instance.ItemClicked(slotIndex,parentInventory);
        isBeingDragged = false;
        //transform.position = startPosition;
        //UIManager.instance.ItemClicked(slotIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(UIManager.instance.dragging)
            imageSprite.color = new Color(0.75f, 1f, 0.75f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isBeingDragged)
            imageSprite.color = new Color(1f, 1f, 1f);
    }
}
