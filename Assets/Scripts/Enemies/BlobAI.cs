using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobAI : EnemyAI
{
    protected override void Update()
    {
        if (dying || pausedAfterDamage) return;
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (playerInRange || Vector2.Distance(player.transform.position, transform.position) < aggroRange)
            {
                anim.SetTrigger("InRange");
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed);
                playerInRange = true;
            }
        }
    }
}
