using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] protected BillboardAnimation sprite;
    [SerializeField] private Texture2D tex;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider col;
    [SerializeField] private Projectile projectile;
    [SerializeField] private GameObject ouch;
    public bool current = false;
    public float speed;
    public float attack;
    public float range;
    public float attackCooldown;
    private float lastAttack;
    public float health;
    public float maxHealth;
    public float regen;
    protected Vector3 mov;
    public Transform moveTarget;
    public bool isEnemy;
    protected bool isDead;
    protected bool invincible;

    protected void Start()
    {
        sprite.SetTexture(tex);
        health = maxHealth;
    }

    protected void Update()
    {
        if (isDead || moveTarget == null)
        {
            return;
        }
        if (!isEnemy || Vector3.Distance(moveTarget.position, transform.position) >= range - 1)
        {
            mov = moveTarget.position - transform.position;
            if (mov.x != 0 || mov.z != 0)
            {
                mov = mov.normalized * Time.deltaTime * speed;
            }
        }
        else
        {
            mov = Vector3.zero;
        }
        if (projectile != null &&
            AttackCooldown() == 0 &&
            Vector3.Distance(moveTarget.position, transform.position) < range)
        {
            FireProjectile();
        }
        if (regen > 0)
        {
            UpdateHealth(regen * Time.deltaTime);
        }
        rb.velocity = Vector3.zero;
        transform.position += mov;
    }

    public float AttackCooldown()
    {
        return 1 - Mathf.Clamp01((Time.time - lastAttack) / attackCooldown);
    }

    protected void FireProjectile()
    {
        lastAttack = Time.time;
        Instantiate(projectile, transform.position, transform.rotation).Init(this);
    }

    protected void FlashDamage()
    {
        sprite.AnimateFlash();
    }

    protected void TakeDamage(Projectile p)
    {
        if (isEnemy == p.isEnemy)
        {
            return;
        }
        p.Hit();
        UpdateHealth(-p.damage);
        FlashDamage();
        OnTakeDamage();
    }

    protected virtual void OnTakeDamage() { }

    protected bool UpdateHealth(float change)
    {
        if (invincible && change < 0)
        {
            return true;
        }
        if (change < 0 && ouch != null)
        {
            ouch.SetActive(false);
            ouch.SetActive(true);
        }
        health += change;
        if (health <= 0)
        {
            Die();
            return false;
        }
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        return true;
    }

    protected virtual void Die()
    {
        sprite.Die();
        isDead = true;
        rb.isKinematic = true;
        col.enabled = false;
        Destroy(gameObject, 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.name)
        {
            case "Projectile":
                Projectile p = other.GetComponent<Collider>().transform.parent.GetComponent<Projectile>();
                TakeDamage(p);
                break;
            case "Dialogue":
                if (current)
                {
                    DialogueCollider dc = other.GetComponent<DialogueCollider>();
                    StartDialogue(dc.def);
                    dc.Trigger();
                }
                break;
            default:
                Debug.Log(other.GetComponent<Collider>().gameObject.name);
                break;
        }
    }

    protected virtual void StartDialogue(DialogueDefinition def)
    {

    }
}