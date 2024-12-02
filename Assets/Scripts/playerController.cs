using UnityEngine;
using UnityEngine.EventSystems;

public class playerController : MonoBehaviour
{
    [SerializeField] Transform playerCamera;  
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;  
    [SerializeField] bool cursorLock = true;  
    [SerializeField] float mouseSensitivity = 3.5f;  
    [SerializeField] float Speed = 6.0f;  
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;  
    [SerializeField] float gravity = -30f;  
    [SerializeField] Transform groundCheck;  
    [SerializeField] LayerMask ground;  

    public float jumpHeight = 6f;  
    public float velocityY;  
    public bool isGrounded;  

    float cameraCap;  
    Vector2 currentMouseDelta;  
    Vector2 currentMouseDeltaVelocity;  

    CharacterController controller;  
    Vector2 currentDir;  
    Vector2 currentDirVelocity;  
    Vector3 velocity;  

    private EventSystem eventSystem;  // Reference to EventSystem

    void Start()
    {
        controller = GetComponent<CharacterController>();  

        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;  
            Cursor.visible = true;  
        }

        // Dynamically find the EventSystem
        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("No EventSystem found in the scene! Please ensure one exists.");
        }
    }

    void Update()
    {
        // Check if the EventSystem is active and the pointer is over a UI element
        if (eventSystem != null && eventSystem.IsPointerOverGameObject())
        {
            return; // Skip player input if interacting with UI
        }

        UpdateMouse();  
        UpdateMove();   
    }

    void UpdateMouse()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); 

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);  

        cameraCap -= currentMouseDelta.y * mouseSensitivity;  
        cameraCap = Mathf.Clamp(cameraCap, -90.0f, 90.0f);  

        playerCamera.localEulerAngles = Vector3.right * cameraCap;  
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);  
    }

    void UpdateMove()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.5f, ground);  

        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));  
        targetDir.Normalize();  

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);  

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * Speed + Vector3.up * velocityY;  

        controller.Move(velocity * Time.deltaTime);  

        if (isGrounded)
        {
            if (velocityY < 0)
            {
                velocityY = 0;
            }

            if (Input.GetButtonDown("Jump"))
            {
                velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);  
            }
        }

        if (!isGrounded)
        {
            velocityY += gravity * 2f * Time.deltaTime;  

            if (controller.velocity.y < -20f)
            {
                velocityY = -20f; 
            }
        }
    }
}
