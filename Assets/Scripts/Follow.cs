using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform toFollow;
    public bool x, y, z;

    private Vector3 startOffset;

    private  void Start() 
    {
        startOffset = transform.position - toFollow.position;
    }

    void Update()
    {
        Vector3 pos = toFollow.position + startOffset;
        transform.position = new Vector3(x ? pos.x : transform.position.x, y ? pos.y : transform.position.y, z ? pos.z : transform.position.z);
    }
}
