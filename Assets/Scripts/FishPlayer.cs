using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishPlayer : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float turnSpeed = 1f;
    void Update()
    {
        if(Keyboard.current.wKey.isPressed)
        {
            transform.forward -= Vector3.up * turnSpeed * Time.deltaTime;
        }
        if(Keyboard.current.aKey.isPressed)
        {
            transform.forward -= transform.right * turnSpeed * Time.deltaTime;
        }
        if(Keyboard.current.sKey.isPressed)
        {
            transform.forward += Vector3.up * turnSpeed * Time.deltaTime;
        }
        if(Keyboard.current.dKey.isPressed)
        {
            transform.forward += transform.right * turnSpeed * Time.deltaTime;
        }

        transform.position -= transform.forward * speed * Time.deltaTime;
    }
}
