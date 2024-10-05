using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    //SerializeField tag just allows variable that we arwe declaring to show up as a property in unity and can be edited directly through Unity without having to reopen the script a thousand times

    [SerializeField] Transform playerCamera;  //Transform is a property in Unity that refers to position, rotation and scale, of a GameObject. Every GameObject has a Tranform component all we are doing here is saying that playerCamera is going to point to a transform component.

    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;  // Controls how smoothly the mouse movement is applied. The Range also allows for a slider in our input box back in Unity from the range of 0.0f to 0.5f 

    [SerializeField] bool cursorLock = true;  // Whether the cursor should be locked in the middle of the screen (FPS games typically do this).
    [SerializeField] float mouseSensitivity = 3.5f;  // How sensitive the mouse movement is for looking around.
    [SerializeField] float Speed = 6.0f;  // Movement speed of the player.
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;  // Controls how smoothly the player's movement changes direction.
    [SerializeField] float gravity = -30f;  // The strength of gravity applied to the player.
    [SerializeField] Transform groundCheck;  // groundCheck is a Transform variable meaning it also will refer to the positon, rotation and scale, of the GameObject we place here 
    [SerializeField] LayerMask ground;  // LayerMask variable created that will allow in the inspector to choopse which layers are considered "ground"

    public float jumpHeight = 6f;  // How high the player can jump.
    public float velocityY;  // Tracks the player's vertical velocity (for jumping and falling).
    public bool isGrounded;  // Whether the player is currently on the ground.

    float cameraCap;  // This prevents the camera from looking too far up or down.
    Vector2 currentMouseDelta;  // Tracks the current movement of the mouse.
    Vector2 currentMouseDeltaVelocity;  // Tracks the velocity of mouse smoothing for a smoother camera movement.
    
    CharacterController controller;  // Reference to the CharacterController component, which handles player movement and collision.
    Vector2 currentDir;  // The current direction the player is moving in (x and z axes).
    Vector2 currentDirVelocity;  // Velocity of movement smoothing.
    Vector3 velocity;  // The total velocity applied to the player (x, y, z).
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();  // Gets a reference to the CharacterController component attached to the player.

        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;  // Locks the cursor to the center of the screen so the player can look around without the cursor leaving the game window.
            Cursor.visible = true;  // Makes the cursor visible if it is locked in the center of the screeen though the line before takes precedence and it wont be visible
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouse();  // Calls the function to handle mouse movement (camera looking around).
        UpdateMove();   // Calls the function to handle player movement (walking, jumping, etc.).
    }

    void UpdateMouse()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); //Input.GetAxis are unitys built in functions that read player mouse movement

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);  // Smooths the mouse movement over time.

        cameraCap -= currentMouseDelta.y * mouseSensitivity;  // Adjusts the camera's vertical rotation based on mouse movement.
        cameraCap = Mathf.Clamp(cameraCap, -90.0f, 90.0f);  // Limits the vertical camera rotation to prevent the player from looking too far up or down.

        playerCamera.localEulerAngles = Vector3.right * cameraCap;  // Rotates the camera based on the vertical mouse movement.
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);  // Rotates the player based on horizontal mouse movement.
    }

    void UpdateMove()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, ground);  // Checks if the player is on the ground by creating a small sphere at the ground check position.

        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));  // Reads raw input for horizontal (A/D or Left/Right) and vertical (W/S or Up/Down) movement.
        targetDir.Normalize();  // Ensures movement input has a consistent magnitude.

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);  // Smooths out player movement direction changes.

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * Speed + Vector3.up * velocityY;  // Combines movement and gravity to create the total velocity.

        controller.Move(velocity * Time.deltaTime);  // Moves the player using the CharacterController, accounting for the calculated velocity.


        if(isGrounded) //Checks if the player is touching the ground
        {
           if(velocityY < 0) //Checks if the player has a negative y velocity (AKA if the player fell before they hit the ground)
           {
                velocityY = 0; //If the player's y velocity is negative while grounded, it will turn to 0
           }

            if(Input.GetButtonDown("Jump"))//Checks if the player presses the jump button
            {
                velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);  // Calculates the jump velocity using physics (gravity and jump height).
            }
        }


        if (!isGrounded ) //Checks if the player is not touching the ground
        {
            velocityY += gravity * 2f * Time.deltaTime;  // Applies gravity to the vertical velocity (pulling the player down) while player is not touching the ground

            if(controller.velocity.y < -20f) // If the player is falling (not grounded and falling fast), reset the velocityY to a default falling value.
            {
                velocityY = -20f; //The maximum falling velocity a player can reach
            }
            
        }
    }


}
