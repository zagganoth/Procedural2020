using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthbar : MonoBehaviour
{
    [SerializeField]
    GameObject positive;
    [SerializeField]
    GameObject negative;
    public float baseHealth;
    float baseScaleX;
    float baseScaleY;
    float curHealth;
    public bool dead;
    // Start is called before the first frame update
    private void Start()
    {
        baseScaleX = positive.gameObject.transform.localScale.x;
        baseScaleY = positive.gameObject.transform.localScale.y;
        curHealth = baseHealth;
    }
    public void takeDamage(float damage)
    {
        curHealth = Mathf.Max(0,curHealth-damage);
        if (curHealth <= 0)
        {
            dead = true;
            return;
        }
        positive.gameObject.transform.localScale = new Vector3(positive.gameObject.transform.localScale.x - ((damage / baseHealth) * baseScaleX), baseScaleY);
    }
}
