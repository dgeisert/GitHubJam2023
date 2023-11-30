using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public GameObject deathParticle;
    public float speed, maxSpeed, minSpeed;
    public float dx, dy, dz;
    public float doMove = 0;
    public bool pauses = false, manual;
    public List<Boid> neighbors;
    public BoidFlock flock;
    public float x
    {
        get
        {
            return transform.position.x;
        }
    }
    public float y
    {
        get
        {
            return transform.position.y;
        }
    }
    public float z
    {
        get
        {
            return transform.position.z;
        }
    }
    public Vector3 xyz
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
        }
    }
    public Vector3 visualNearbyCenter;

    public void Move()
    {
        Vector3 move = new Vector3(dx, dy, dz);
        
        if (move.sqrMagnitude > 25 || doMove >= 0)
        {
            move = Mathf.Clamp(move.magnitude, minSpeed, maxSpeed) * move.normalized;
            dx = move.x;
            dy = move.y;
            dz = move.z;
            transform.position += move * Time.deltaTime * speed;
            transform.LookAt(transform.position - move);
        }

        //have frequent pauses in the movement instead of being smooth
        /*
        if (pauses)
        {
            doMove += Time.deltaTime;
            if (doMove >= 1)
            {
                doMove = -2 * Random.value;
            }
        }
        */
    }

    public void Avoid(Vector3 pos, float range, float turn, float modifier)
    {
        //if avoider is near move away
        Vector3 myPos = transform.position;
        float dist = Vector3.Distance(pos, myPos);
        if (dist < range)
        {
            Vector3 dir = (new Vector3(myPos.x - pos.x, myPos.y - pos.y, myPos.z - pos.z)).normalized;
            dx += (range - dist) / range * dir.x * turn * modifier;
            dy += (range - dist) / range * dir.y * turn * modifier;
            dz += (range - dist) / range * dir.z * turn * modifier;
        }
    }

    public void Die()
    {
        Destroy(GameObject.Instantiate(deathParticle, transform.position, transform.rotation), 10f);
        flock.RemoveBoid(this);
        Destroy(gameObject);
    }
}