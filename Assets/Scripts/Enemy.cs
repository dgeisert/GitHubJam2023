using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public EnemyManager em;
    public float expValue = 1;
    private void Start()
    {
        base.Start();
        isEnemy = true;
    }

    private void Update()
    {
        base.Update();
    }

    protected override void Die()
    {
        em.Die(this);
        base.Die();
    }
}