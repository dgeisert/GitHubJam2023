using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidFlock : MonoBehaviour
{
    [SerializeField] private int neighborCount = 6;
    [SerializeField] private Boid boid;
    [SerializeField] private bool y = false;
    [SerializeField] private int randomSpawnCount;
    [SerializeField] private Vector3 randomSpawnRange;
    private List<Boid> boids;
    [SerializeField] private List<BoidAvoid> boidAvoids;
    [SerializeField] private Transform centerMarker;
    [SerializeField] private Transform currentCenter;

    private Vector3 center;

    [SerializeField] private float speed = 0.2f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float minSpeed = 0f;

    [SerializeField] private float avoidRange = 2f;
    [SerializeField] private float visualRange = 15f;
    [SerializeField] private float avoidTurn = 0.05f;
    [SerializeField] private float centerTurn = 0.005f;
    [SerializeField] private float globalCenterBias = 0.0002f;
    [SerializeField] private float angularCoherence = 0.1f;

    [SerializeField] private float avoiderModifier = 2;

    [SerializeField] private Vector3 startVelocity;
    [SerializeField] private float startRandom;

    public void Init()
    {
        if (centerMarker == null)
        {
            centerMarker = transform;
        }
        boids = new List<Boid>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Boid b = transform.GetChild(i).GetComponent<Boid>();
            b.flock = this;
            b.speed = speed;
            b.maxSpeed = maxSpeed;
            b.minSpeed = minSpeed;
            boids.Add(b);
        }

        //init random boid
        for (int i = 0; i < randomSpawnCount; i++)
        {
            Boid b = Instantiate(
                boid,
                transform.position +
                new Vector3(
                    (Random.value - 0.5f) * randomSpawnRange.x,
                    (Random.value - 0.5f) * randomSpawnRange.y,
                    (Random.value - 0.5f) * randomSpawnRange.z),
                Quaternion.Euler(0, Random.value * 360, 0),
                transform);
            b.flock = this;
            b.dx = startVelocity.x + startRandom * Random.value;
            b.dy = y ? startVelocity.y + startRandom * Random.value : 0;
            b.dz = startVelocity.z + startRandom * Random.value;
            b.speed = speed;
            b.maxSpeed = maxSpeed;
            b.minSpeed = minSpeed;
            boids.Add(b);
        }
        foreach (var b in boids)
        {
            b.neighbors = new List<Boid>();
            for (int i = 0; i < neighborCount; i++)
            {
                b.neighbors.Add(RandomBoid(b));
            }
        }

        boid.dz = 10;
    }

    public void UpdateBoids()
    {
        Vector3 center = Vector3.zero;
        foreach (var b in boids)
        {
            Vector3 bpos = b.transform.position;
            center += bpos;
            float i = 0;
            b.visualNearbyCenter = Vector3.zero;
            float maxDist = 0;
            Boid furthest = null;
            foreach (var b2 in b.neighbors)
            {
                Vector3 b2pos = b2.transform.position;
                float dist = Vector3.Distance(bpos, b2pos);
                //if boid is nearby add it to the group you are moving with
                if (dist > maxDist)
                {
                    furthest = b2;
                    maxDist = dist;
                }
                if (dist < visualRange)
                {
                    i++;
                    b.visualNearbyCenter += b2.transform.position;

                    //change my movement to be closer to the movement direction and speed of the other boid
                    b.dx += b2.dx * angularCoherence * 0.01f;
                    b.dy += b2.dy * angularCoherence * 0.01f;
                    b.dz += b2.dz * angularCoherence * 0.01f;

                    //move towards the user controlled boid
                    if (b2.manual)
                    {
                        b.dx += (b2pos.x - bpos.x) * centerTurn;
                        b.dy += (b2pos.y - bpos.y) * centerTurn;
                        b.dz += (b2pos.z - bpos.z) * centerTurn;
                    }
                }
                //if boid is very close move away from it
                if (dist < avoidRange)
                {
                    b.Avoid(b2pos, avoidRange, avoidTurn, 1);
                }
            }

            //handle avoids
            foreach (var ba in boidAvoids)
            {
                b.Avoid(ba.transform.position, ba.distance, avoidTurn, avoiderModifier);
            }

            //remove the furthest neighbor and grab a random one from the flock
            if (neighborCount > 0)
            {
                //b.neighbors.Remove(furthest);
                //b.neighbors.Add(RandomBoid(b));
            }
            //if there are boid in range move towards the center of them
            if (i > 0)
            {
                b.visualNearbyCenter /= i;
                b.dx += (b.visualNearbyCenter.x - b.x) * centerTurn;
                b.dy += (b.visualNearbyCenter.y - b.y) * centerTurn;
                b.dz += (b.visualNearbyCenter.z - b.z) * centerTurn;
            }
            else
            {
                //if alone head towards random member of flock
                b.dx += (b.x - boids[Mathf.FloorToInt(Random.value * boids.Count)].x) * centerTurn / 2;
                b.dy += (b.y - boids[Mathf.FloorToInt(Random.value * boids.Count)].y) * centerTurn / 2;
                b.dz += (b.z - boids[Mathf.FloorToInt(Random.value * boids.Count)].z) * centerTurn / 2;
            }

            //global center bias
            b.dx += (centerMarker.position.x - b.x) * globalCenterBias;
            b.dy += (centerMarker.position.y - b.y) * globalCenterBias;
            b.dz += (centerMarker.position.z - b.z) * globalCenterBias;

            b.Move();
        }

        center /= boids.Count;
        if(currentCenter != null)
        {
            currentCenter.position = center;
        }
    }

    public void AddBoid(Boid b)
    {
        if (!boids.Contains(b))
        {
            boids.Add(b);
        }
    }

    public void RemoveBoid(Boid b)
    {
        if (boids.Contains(b))
        {
            boids.Remove(b);
        }
    }

    public Boid RandomBoid(Boid b)
    {
        Boid b2 = boids[Mathf.FloorToInt(Random.value * boids.Count)];
        if (b == b2 || b.neighbors.Contains(b2))
        {
            return RandomBoid(b);
        }
        return b2;
    }
}