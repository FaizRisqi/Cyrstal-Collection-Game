using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform target; // Player transform
    [SerializeField] private float distance = 5f;
    [SerializeField] private float height = 2f;
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float smoothSpeed = 10f;

    [Header("Rotation Limits")]
    [SerializeField] private float minVerticalAngle = -30f;
    [SerializeField] private float maxVerticalAngle = 60f;

    private float currentX = 0f;
    private float currentY = 20f; // Start with slight downward angle

    void Start()
    {
        if (target == null)
        {
            // Try to find player automatically
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Get mouse input
        currentX += Input.GetAxis("Mouse X") * mouseSensitivity;
        currentY -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Clamp vertical rotation
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);

        // Calculate camera position
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 offset = new Vector3(0, height, -distance);
        Vector3 desiredPosition = target.position + rotation * offset;

        // Smooth camera movement
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        // Look at target
        transform.LookAt(target.position + Vector3.up * height);
    }
}