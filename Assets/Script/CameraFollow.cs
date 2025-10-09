using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The object to follow (e.g., player)
    public float smoothSpeed = 0.125f;
    public Vector2 minBounds; // Minimum X and Y (left/bottom wall)
    public Vector2 maxBounds; // Maximum X and Y (right/top wall)

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Clamp camera position within bounds
        float clampedX = Mathf.Clamp(smoothedPosition.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(smoothedPosition.y, minBounds.y, maxBounds.y);
        transform.position = new Vector3(clampedX, clampedY, smoothedPosition.z);
    }
}
