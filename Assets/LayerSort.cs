using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSort : MonoBehaviour
{
    [SerializeField]
    private int sortingOrderBase = 5000;
    [SerializeField]
    bool runOnce = true;
    private SpriteRenderer renderer;
    [SerializeField]
    private int offset = 0;
    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();

    }
    private void Start()
    {
        if (!runOnce)
        {
            StartCoroutine(layerSort());
        }
        else
        {
            changeOrder();
        }
    }
    private IEnumerator layerSort()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.1f);
            changeOrder();
        }

    }
    private void changeOrder()
    {
        Debug.Log(transform.position.y);
        renderer.sortingOrder = (int)(sortingOrderBase - transform.position.y - offset);
    }
}
