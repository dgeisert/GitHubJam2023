using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorPlacement : MonoBehaviour
{
    public static CursorPlacement Instance;
    private Camera cam;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        transform.position = ray.origin - ray.direction * ray.origin.y / ray.direction.y;
    }
}
