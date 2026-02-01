using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
   [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -5);
    
    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float smoothSpeed = 10f;
    
    [Header("Rotation Limits")]
    [SerializeField] private float minVerticalAngle = -40f;
    [SerializeField] private float maxVerticalAngle = 80f;
    
    private float rotationX = 0f;
    private float rotationY = 0f;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Initialize rotation based on current camera angle
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;
    }
    
    private void Update()
    {
        if (target == null) return;
        
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Update rotation
        rotationX += mouseX;
        rotationY -= mouseY;
        rotationY = Mathf.Clamp(rotationY, minVerticalAngle, maxVerticalAngle);
        
        // Unlock cursor with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    private void LateUpdate()
    {
        if (target == null) return;
        
        // Calculate rotation
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
        
        // Calculate desired position
        Vector3 desiredPosition = target.position + rotation * offset;
        
        // Smoothly move camera
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        // Look at target
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
