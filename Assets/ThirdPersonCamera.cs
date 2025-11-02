using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float followDistance = 5.0f;
    [SerializeField] private float followHeight = 2.0f;
    [SerializeField] private float followSmoothing = 0.1f;
    
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSensitivity = 2.0f;
    [SerializeField] private float minVerticalAngle = -30.0f;
    [SerializeField] private float maxVerticalAngle = 60.0f;
    
    [Header("Collision Settings")]
    [SerializeField] private float collisionRadius = 0.2f;
    [SerializeField] private LayerMask collisionLayers;
    
    private Vector3 cameraOffset;
    private float currentYRotation;
    private float currentXRotation;
    
    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("No target assigned to ThirdPersonCamera");
            return;
        }
        
        // Initialize rotation values
        Vector3 angles = transform.eulerAngles;
        currentYRotation = angles.y;
        currentXRotation = angles.x;
        
        // Initial offset
        cameraOffset = new Vector3(0, followHeight, -followDistance);
    }
    
    private void LateUpdate()
    {
        if (target == null) return;
        
        // Handle rotation input
        currentYRotation += Input.GetAxis("Mouse X") * rotationSensitivity;
        currentXRotation -= Input.GetAxis("Mouse Y") * rotationSensitivity;
        
        // Clamp vertical rotation
        currentXRotation = Mathf.Clamp(currentXRotation, minVerticalAngle, maxVerticalAngle);
        
        // Calculate rotation and target position
        Quaternion rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);
        Vector3 targetPosition = target.position;
        
        // Calculate desired camera position
        Vector3 desiredPosition = targetPosition + rotation * cameraOffset;
        
        // Handle camera collision
        RaycastHit hit;
        Vector3 cameraDirection = (desiredPosition - targetPosition).normalized;
        float adjustedDistance = cameraOffset.magnitude;
        
        if (Physics.SphereCast(targetPosition, collisionRadius, cameraDirection, out hit, followDistance, collisionLayers))
        {
            // Adjust distance if there's an obstacle
            adjustedDistance = hit.distance * 0.8f; // Add a small buffer
        }
        
        Vector3 collisionAdjustedPosition = targetPosition + cameraDirection * adjustedDistance;
        
        // Apply smoothing for camera movement
        transform.position = Vector3.Lerp(transform.position, collisionAdjustedPosition, followSmoothing);
        
        // Look at a point slightly above the target
        Vector3 lookAtPosition = targetPosition + Vector3.up * (followHeight * 0.5f);
        transform.LookAt(lookAtPosition);
    }
}