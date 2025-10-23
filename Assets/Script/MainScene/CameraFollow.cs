using UnityEngine;

/// <summary>
/// Optimized 2D Camera Follow with optional smoothing, bounds, and offset.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow Settings")]
    public bool useSmoothing = true;
    [Tooltip("Lower = snappier follow.")]
    public float smoothTime = 0.12f;
    public Vector2 offset = new Vector2(0f, 2f); // offset from target

    [Header("Bounds")]
    public bool useBounds = false;
    public Vector2 minLimits = new Vector2(-10f, -5f);
    public Vector2 maxLimits = new Vector2(50f, 10f);

    [Header("Debug")]
    public bool drawGizmos = true;

    private Camera _cam;
    private Vector3 _velocity = Vector3.zero;
    private float _halfWidth;
    private float _halfHeight;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        if (!_cam.orthographic)
            Debug.LogWarning("CameraFollow: Camera is not orthographic. Behavior may differ in perspective mode.");

        _halfHeight = _cam.orthographicSize;
        _halfWidth = _halfHeight * _cam.aspect;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Desired position with offset
        Vector3 desired = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);

        // Apply bounds if enabled
        if (useBounds)
        {
            float minX = minLimits.x + _halfWidth;
            float maxX = maxLimits.x - _halfWidth;
            float minY = minLimits.y + _halfHeight;
            float maxY = maxLimits.y - _halfHeight;

            desired.x = Mathf.Clamp(desired.x, minX, maxX);
            desired.y = Mathf.Clamp(desired.y, minY, maxY);
        }

        // Smooth or direct movement
        Vector3 nextPos = useSmoothing
            ? Vector3.SmoothDamp(transform.position, desired, ref _velocity, Mathf.Max(0.0001f, smoothTime), Mathf.Infinity, Time.deltaTime)
            : desired;

        nextPos.z = transform.position.z;
        transform.position = nextPos;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos || !useBounds) return;

        // Draw world bounds
        Gizmos.color = Color.yellow;
        Vector3 center = (Vector3)((minLimits + maxLimits) * 0.5f);
        Vector3 size = new Vector3(maxLimits.x - minLimits.x, maxLimits.y - minLimits.y, 0f);
        Gizmos.DrawWireCube(center, size);

        // Draw camera view rectangle
        if (_cam != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 camCenter = transform.position;
            Vector3 camSize = new Vector3(_halfWidth * 2f, _halfHeight * 2f, 0f);
            Gizmos.DrawWireCube(camCenter, camSize);
        }
    }
}
