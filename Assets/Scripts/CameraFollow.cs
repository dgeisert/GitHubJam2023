using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform toFollow;
    [SerializeField] private float followStrength = 0.2f;
    [SerializeField] private float followDistance = 10f;
    void Update()
    {
        transform.LookAt(toFollow);
        transform.position = Vector3.Lerp(
            transform.position, 
            ((transform.position - toFollow.position).normalized + toFollow.forward) * 0.5f * followDistance + toFollow.position, 
            followStrength);
    }
}
