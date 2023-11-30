using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [SerializeField] private List<BoidFlock> flocks;

    private void Start()
    {
        if (flocks == null)
        {
            flocks = new List<BoidFlock>();
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            BoidFlock f = transform.GetChild(i).GetComponent<BoidFlock>();
            if (f != null && !flocks.Contains(f))
            {
                flocks.Add(f);
            }
        }
        foreach (var flock in flocks)
        {
            if (flock.gameObject.activeSelf)
            {
                flock.Init();
            }
        }
    }

    private void Update()
    {
        foreach (var flock in flocks)
        {
            if (flock.gameObject.activeSelf)
            {
                flock.UpdateBoids();
            }
        }
    }
}