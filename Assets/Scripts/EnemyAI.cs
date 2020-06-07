using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    protected float aggroRange;
    [SerializeField]
    protected float speed;
    protected Animator anim;
    protected bool playerInRange;
    public healthbar health;
    protected bool dying;
    protected bool onCooldown;
    [SerializeField]
    public float damageCooldownTime;
    [SerializeField]
    public float deathAnimationTime;
    [SerializeField]
    public float damagePauseTime;
    [SerializeField]
    public bool pausedAfterDamage;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        playerInRange = false;
        health = GetComponentInChildren<healthbar>();
        dying = false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }
    public virtual void takeDamage(float damage)
    {
        if (onCooldown) return;
        health.takeDamage(damage);
        if (health.dead)
        {
            anim.SetTrigger("explode");
            StartCoroutine(waitAndDie());
        }
        else
        {
            Debug.Log("Hurting!");
            anim.SetTrigger("Hurt");
            onCooldown = true;
            StartCoroutine(coolDown());
            pausedAfterDamage = true;
            StartCoroutine(waitBeforeMove());
        }
    }
    public IEnumerator waitAndDie()
    {
        dying = true;
        yield return new WaitForSeconds(deathAnimationTime);
        Destroy(gameObject);
    }
    public IEnumerator coolDown()
    {
        if (!onCooldown) yield return null;
        yield return new WaitForSeconds(damageCooldownTime);
        onCooldown = false;
    }
    
    public IEnumerator waitBeforeMove()
    {
        if (!pausedAfterDamage) yield return null;
        yield return new WaitForSeconds(damagePauseTime);
        pausedAfterDamage = false;
    }
}
