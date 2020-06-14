using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : LeftClickable
{
    ParticleSystem pc;
    private void Awake()
    {
        pc = GetComponent<ParticleSystem>();
    }
    [SerializeField]
    public ItemObject dropItem;
    [SerializeField]
    public int amount;
    public void CollectItem(EventManager.OnLeftClickArgs lef)
    {
        lef.invRef.AddItemToInventory(dropItem);
        if(--amount <= 0)
        {
            Destroy(gameObject);
        }
        pc.Play();
    }
}
