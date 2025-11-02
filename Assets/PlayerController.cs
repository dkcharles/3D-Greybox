using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    // Character controller component
    private CharacterController controller;
    private Camera mainCamera;
    
    // Movement parameters
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float sprintSpeed = 8.0f;
    [SerializeField] private float rotationSpeed = 10.0f;
    
    // Jump parameters
    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float fallMultiplier = 2.5f;
    
    // State variables
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraTransform;
    
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        
        if (mainCamera != null)
        {
            cameraTransform = mainCamera.transform;
        }
        
        // Lock and hide cursor for camera control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Update()
    {
        // Check if character is grounded
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -0.5f; // Small downward force to ensure grounding
        }
        
        // Get input for movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // Calculate movement direction relative to camera
        Vector3 direction = Vector3.zero;
        if (cameraTransform != null)
        {
            // Create movement vector relative to camera orientation (excluding vertical component)
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();
            
            direction = forward * vertical + right * horizontal;
        }
        else
        {
            // Fallback if camera reference isn't available
            direction = new Vector3(horizontal, 0.0f, vertical);
        }
        
        // Normalize movement vector to prevent faster diagonal movement
        if (direction.magnitude > 1f)
        {
            direction.Normalize();
        }
        
        // Handle character rotation to face movement direction
        if (direction != Vector3.zero)
        {
            // Calculate rotation to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        // Apply movement with appropriate speed
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        controller.Move(direction * Time.deltaTime * currentSpeed);
        
        // Handle jumping
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }
        
        // Apply gravity with enhanced falling
        if (playerVelocity.y < 0)
        {
            // Apply stronger gravity when falling for better feel
            playerVelocity.y += gravityValue * fallMultiplier * Time.deltaTime;
        }
        else
        {
            // Standard gravity for rising
            playerVelocity.y += gravityValue * Time.deltaTime;
        }
        
        // Apply vertical movement
        controller.Move(playerVelocity * Time.deltaTime);
    }
}