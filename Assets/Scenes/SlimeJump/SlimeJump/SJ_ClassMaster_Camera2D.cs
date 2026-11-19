using UnityEngine;

public class SJ_ClassMaster_Camera2D : MonoBehaviour
{
    // 00 - REFERENCES
    // Assign the player and set the camera offset.
    // Assignez le joueur et définissez le décalage de la caméra.
    [Header("00 | References / Références")]
    public Transform target;
    public Vector2 offset = new Vector2(1f, 1f);

    // 01 - FOLLOW AXES
    // Choose whether the camera follows horizontal and vertical movement.
    // Choisissez si la caméra suit les déplacements horizontaux et verticaux.
    [Header("01 | Follow Axes / Axes de suivi")]
    public bool followX = true;
    public bool followY = true;

    // 02 - CAMERA DELAY
    // Smooth camera movement to create a sense of acceleration and momentum.
    // Adoucissez le mouvement de la caméra pour ressentir l'accélération et l'inertie.
    [Header("02 | Camera Delay / Retard de la caméra")]
    public bool useSmoothing = true;
    [Min(0.01f)] public float horizontalSmoothTime = 0.3f;
    [Min(0.01f)] public float verticalSmoothTime = 0.18f;

    // 03 - SPEED LOOK AHEAD
    // Optionally show more space in front of the player as horizontal speed increases.
    // Affichez plus d'espace devant le joueur lorsque sa vitesse horizontale augmente.
    [Header("03 | Speed Look Ahead / Anticipation de vitesse")]
    public bool useSpeedLookAhead = false;
    [Min(0f)] public float lookAheadPerSpeed = 0.15f;
    [Min(0f)] public float maxLookAhead = 2f;

    private Rigidbody2D targetRigidbody;
    private float cameraVelocityX;
    private float cameraVelocityY;
    private float originalZ;

    // 04 - INITIALIZATION
    // Save the camera depth and find the player's Rigidbody2D.
    // Conservez la profondeur de la caméra et trouvez le Rigidbody2D du joueur.
    private void Start()
    {
        originalZ = transform.position.z;

        if (target != null)
            targetRigidbody = target.GetComponent<Rigidbody2D>();
    }

    // 05 - CAMERA FOLLOW
    // Follow the player after movement updates, with optional delay and look ahead.
    // Suivez le joueur après son déplacement, avec retard et anticipation facultatifs.
    private void LateUpdate()
    {
        if (target == null)
            return;

        float desiredX = target.position.x + offset.x;
        float desiredY = target.position.y + offset.y;

        // 06 - SPEED LOOK AHEAD CALCULATION
        // Add an offset based on horizontal speed without exceeding the limit.
        // Ajoutez un décalage selon la vitesse horizontale, sans dépasser la limite.
        if (useSpeedLookAhead && targetRigidbody != null)
        {
            float lookAhead = Mathf.Clamp(
                targetRigidbody.linearVelocity.x * lookAheadPerSpeed,
                -maxLookAhead,
                maxLookAhead
            );

            desiredX += lookAhead;
        }

        // 07 - HORIZONTAL FOLLOW
        // Smooth the X position, or follow immediately when smoothing is disabled.
        // Lissez la position X ou suivez immédiatement si le lissage est désactivé.
        float newX = transform.position.x;

        if (followX)
        {
            newX = useSmoothing
                ? Mathf.SmoothDamp(transform.position.x, desiredX,
                    ref cameraVelocityX, horizontalSmoothTime)
                : desiredX;
        }

        // 08 - VERTICAL FOLLOW
        // Smooth the Y position independently from horizontal movement.
        // Lissez la position Y indépendamment du déplacement horizontal.
        float newY = transform.position.y;

        if (followY)
        {
            newY = useSmoothing
                ? Mathf.SmoothDamp(transform.position.y, desiredY,
                    ref cameraVelocityY, verticalSmoothTime)
                : desiredY;
        }

        // 09 - APPLY CAMERA POSITION
        // Keep the original Z position so the 2D camera stays in front of the scene.
        // Gardez la position Z d'origine pour que la caméra 2D reste devant la scène.
        transform.position = new Vector3(newX, newY, originalZ);
    }
}
