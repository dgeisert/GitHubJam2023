using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    private bool triggered = false;
    [SerializeField] private Texture2D triggeredTex;
    [SerializeField] private Transform healthBar;
    [SerializeField] private bool finalBoss;
    [SerializeField] private MenuManager menu;

    private void Start()
    {
        base.Start();
        isEnemy = true;
        if (finalBoss)
        {
            sprite.isStatic = true;
            StartCoroutine(Sleep());
        }
    }
    IEnumerator Sleep()
    {
        Flytext.CreateFlytext(transform.position + new Vector3(-0.5f, 2, 1), "Zzz", Color.white, 8, 3, 3);
        yield return new WaitForSeconds(2);
        if (!triggered)
        {
            StartCoroutine(Sleep());
        }
    }
    void Update()
    {
        if (!triggered)
        {
            return;
        }
        healthBar.localScale = new Vector3(health / maxHealth, 0.05f, 1);
        base.Update();
    }

    public void Trigger()
    {
        if (triggered)
        {
            return;
        }
        sprite.isStatic = false;
        sprite.SetTexture(triggeredTex);
        triggered = true;
    }

    protected override void Die()
    {
        base.Die();
        menu.ShowMenu("Victory");
    }

    protected override void OnTakeDamage()
    {
        Trigger();
    }
}