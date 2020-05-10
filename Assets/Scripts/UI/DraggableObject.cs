using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggableObject : MonoBehaviour
{
    public Image img;
    public ItemObject item;
    public static DraggableObject instance;
    public bool dragging;
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
        gameObject.SetActive(false);
    }
    public void Reset()
    {
        img.sprite = null;
        item = null;
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        //Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //transform.position = new Vector3(pos.x, pos.y, 0);
        transform.position = Input.mousePosition;
    }
}
