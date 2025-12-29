using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    
    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float gravity = -18f;
    
    [Header("Camera")]
    [SerializeField] private Transform playerCamera;
    
    private CharacterController controller;
    private float verticalVelocity;
    private float turnSmoothVelocity;
    
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        
        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }
    }
    
    private void Update()
    {
        HandleMovement();
        HandleJump();
        ApplyGravity();
    }
    
    private void HandleMovement()
    {
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Calculate movement direction relative to camera
        Vector3 cameraForward = playerCamera.forward;
        Vector3 cameraRight = playerCamera.right;
        
        // Flatten to horizontal plane
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        Vector3 moveDirection = cameraRight * horizontal + cameraForward * vertical;
        
        // Determine speed
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        
        // Move character
        Vector3 move = moveDirection.normalized * currentSpeed;
        controller.Move(move * Time.deltaTime + new Vector3(0, verticalVelocity * Time.deltaTime, 0));
        
        // Rotate character to face movement direction
        if (moveDirection.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }
    
    private void HandleJump()
    {
        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    
    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }
}
