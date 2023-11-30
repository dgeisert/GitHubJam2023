using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleCameraMovement : MonoBehaviour
{
    public static SimpleCameraMovement Instance;
    public float movementSpeed = 5f; // Speed at which the camera moves
    public float zoomSpeed = 5f; // Speed at which the camera zooms
    public float smoothing = 0.1f; // Smoothing factor for starting and stopping
    public float minZoom = 0.3f; // Minimum zoom distance
    public float maxZoom = 3f; // Maximum zoom distance

    private Vector2 moveInput;
    private float zoomInput;
    private Vector3 moveDirection;
    private Vector3 smoothMoveVelocity;
    public Transform target;

    private GameMenu game;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        game = FindObjectOfType<GameMenu>();
    }

    private void Update()
    {
        if (!game.isEnabled || target == null)
        {
            return;
        }
        
        Vector3 center = transform.position - transform.forward * transform.position.y / transform.forward.y;

        // Get input from scroll wheel using UnityEngine.InputSystem
        zoomInput = Mouse.current.scroll.ReadValue().y * zoomSpeed;
        if (transform.position.y > maxZoom)
        {
            if (transform.position.y > maxZoom + 0.5f && smoothMoveVelocity.y > 0)
            {
                smoothMoveVelocity = new Vector3(0, 0, 0);
            }
            zoomInput = 1;
        }
        else if (transform.position.y < minZoom)
        {
            if (transform.position.y < minZoom - 0.5f && smoothMoveVelocity.y < 0)
            {
                smoothMoveVelocity = new Vector3(0, 0, 0);
            }
            zoomInput = -1;
        }

        // Calculate the movement direction
        moveDirection = (target.position - center) + transform.forward * zoomInput;

        // Apply smoothing to starting and stopping
        Vector3 targetVelocity = moveDirection * movementSpeed;
        smoothMoveVelocity = Vector3.Lerp(smoothMoveVelocity, targetVelocity, smoothing);

        // Move the camera
        transform.position += (smoothMoveVelocity * Time.deltaTime);
    }
}