using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobAI : EnemyAI
{
    [SerializeField]
    float aggroRange;
    [SerializeField]
    float speed;
    Animator anim;
    bool playerInRange;
    public healthbar health;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        playerInRange = false;
        health = GetComponentInChildren<healthbar>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(playerInRange || Vector2.Distance(player.transform.position,transform.position) < aggroRange)
            {
                anim.SetTrigger("InRange");
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed);
                playerInRange = true;
            }
        }
    }
    public override void takeDamage(float damage)
    {
        health.takeDamage(damage);
        anim.SetTrigger("Hurt");
    }
}
