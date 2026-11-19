using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class SJ_ClassMaster_Movement2D : MonoBehaviour
{
    // 00 - REFERENCES
    // Position and size of the ground probe.
    // Position et taille du détecteur de sol.
    [Header("00 | References / Références")]
    [Tooltip("Optional / Facultatif : child Transform positioned at the feet / Transform enfant placé aux pieds.")]
    public Transform groundCheckPoint;
    public Vector2 groundCheckOffset = new Vector2(0f, -0.5f);
    public float groundCheckRadius = 0.18f;
    [Tooltip("Required for ground and ceiling checks / Requis pour détecter le sol et le plafond.")]
    public LayerMask groundLayer;

    // 01 - HORIZONTAL MOVEMENT
    // Essential for acceleration, air control, apex speed and auto-run.
    // Essentiel pour l’accélération, le contrôle aérien, le bonus de vitesse au sommet et la course automatique.
    [Header("01 | ESSENTIAL - Movement / Déplacement")]
    [Tooltip("Enables left/right movement / Active le déplacement gauche/droite.")]
    public bool enableHorizontalMovement = true;
    public float maxMoveSpeed = 8f;

    // 02 - ACCELERATION
    // Optional; requires horizontal movement.
    // Facultatif ; nécessite le déplacement horizontal.
    [Header("02 | OPTIONAL - Acceleration / Accélération")]
    [Tooltip("Requires horizontal movement. OFF gives instant speed / Nécessite le déplacement horizontal. Désactivé : vitesse immédiate.")]
    public bool useAcceleration = true;
    public float acceleration = 55f;
    public float groundDeceleration = 70f;
    [Tooltip("Requires acceleration and ground check / Nécessite l'accélération et la détection du sol.")]
    public bool useDifferentAirDeceleration = true;
    public float airDeceleration = 25f;

    // 03 - AIR CONTROL
    // Optional; requires horizontal movement and ground detection.
    // Facultatif ; nécessite le déplacement horizontal et la détection du sol.
    [Header("03 | OPTIONAL - Air Control / Contrôle aérien")]
    [Tooltip("OFF preserves horizontal momentum in the air / Désactivé : conserve l'élan horizontal dans les airs.")]
    public bool useAirControl = true;
    [Range(0f, 1f)] public float airControlMultiplier = 0.75f;

    // 04 - JUMP
    // Essential for platforming; required for coyote time, jump buffering and variable jump height.
    // Essentiel pour les plateformes ; requis pour le temps de grâce, la mémoire du saut et la hauteur de saut variable.
    [Header("04 | ESSENTIAL - Jump / Saut")]
    [Tooltip("Enable jumping with Space / Active le saut avec Espace.")]
    public bool enableJump = true;
    public float jumpForce = 13f;

    // 05 - GROUND CHECK
    // Optional for teaching; turning it off allows jumping in midair.
    // Facultatif pour le cours ; sa désactivation permet de sauter dans les airs.
    [Header("05 | OPTIONAL - Ground Check / Sol")]
    [Tooltip("Required by coyote time and air control / Requis pour le coyote time et le contrôle aérien.")]
    public bool useGroundCheck = true;

    // 06 - CEILING CHECK
    // Optional; stops upward movement when the player touches a ceiling.
    // Facultatif ; arrête la montée lorsque le joueur touche un plafond.
    [Header("06 | OPTIONAL - Ceiling Check / Plafond")]
    [Tooltip("Requires Collider2D and Ground Layer / Nécessite Collider2D et Ground Layer.")]
    public bool useCeilingCheck = false;
    public float ceilingCheckRadius = 0.12f;

    // 07 - COYOTE TIME
    // Allows jumping briefly after leaving a platform.
    // Permet de sauter brièvement après avoir quitté une plateforme.
    [Header("07 | OPTIONAL - Coyote Time / Temps de grâce")]
    [Tooltip("Requires Jump + Ground Check / Nécessite Saut + Détection du sol.")]
    public bool useCoyoteTime = true;
    public float coyoteTime = 0.12f;

    // 08 - JUMP BUFFER
    // Remembers a jump requested just before landing.
    // Mémorise un saut demandé juste avant l’atterrissage.
    [Header("08 | OPTIONAL - Jump Buffer / Mémoire du saut")]
    [Tooltip("Requires Jump + Ground Check / Nécessite Saut + Détection du sol.")]
    public bool useJumpBuffer = true;
    public float jumpBufferTime = 0.12f;

    // 08B - DOUBLE JUMP
    // Allows one additional jump in the air after the first jump.
    // Permet un saut supplémentaire dans les airs après le premier saut.
    [Header("08B | OPTIONAL - Double Jump / Double saut")]
    [Tooltip("Requires Jump + Ground Check / Nécessite Saut + Détection du sol.")]
    public bool useDoubleJump = false;

    // 09 - VARIABLE JUMP HEIGHT
    // Releasing Space early produces a shorter jump.
    // Relâcher Espace plus tôt permet de faire un saut plus court.
    [Header("09 | OPTIONAL - Variable Jump / Saut variable")]
    [Tooltip("Requires Jump + Gravity / Nécessite Saut + Gravité.")]
    public bool useVariableJumpHeight = true;
    public float jumpCutGravityMultiplier = 3f;

    // 10 - CUSTOM GRAVITY
    // Applies gravity through this script instead of the Rigidbody2D component.
    // Applique la gravité dans ce script plutôt que dans le composant Rigidbody2D.
    [Header("10 | ESSENTIAL - Gravity / Gravité")]
    [Tooltip("OFF means no custom gravity, so the player floats / Désactivé : pas de gravité personnalisée, le joueur flotte.")]
    public bool enableGravity = true;
    public float gravityAcceleration = 38f;
    [Tooltip("Requires Ground Check. Keeps the player close to the floor / Nécessite la détection du sol. Garde le joueur près du sol.")]
    public bool useGroundStick = true;
    public float groundStickSpeed = 1.5f;

    // 11 - FALLING AND JUMP APEX
    // Optional adjustments to gravity and horizontal speed near the top of a jump.
    // Ajustements facultatifs de la gravité et de la vitesse horizontale près du sommet du saut.
    [Header("11 | OPTIONAL - Falling / Chute")]
    [Tooltip("Requires Gravity. Faster descent after the jump apex / Nécessite la gravité. Descente plus rapide après le sommet.")]
    public bool useFastFall = false;
    public float fastFallMultiplier = 1.5f;
    [Tooltip("Requires Gravity + Ground Check. Slower falling near the apex / Nécessite Gravité + Détection du sol. Gravité réduite au sommet.")]
    public bool useApexHang = false;
    public float apexSpeedThreshold = 2f;
    [Range(0.05f, 1f)] public float apexGravityMultiplier = 0.5f;
    [Tooltip("Requires Horizontal Movement + Ground Check. Extra horizontal speed near the apex / Nécessite Déplacement horizontal + Détection du sol. Bonus de vitesse au sommet.")]
    public bool useApexSpeedBonus = false;
    public float apexHorizontalBonus = 2f;

    // 12 - MAXIMUM FALL SPEED
    // Optional limit to downward velocity.
    // Limite facultative de la vitesse de chute.
    [Header("12 | OPTIONAL - Max Fall / Chute maximale")]
    [Tooltip("Caps downward speed / Limite la vitesse de chute.")]
    public bool useMaxFallSpeed = true;
    public float maxFallSpeed = 20f;

    // 13 - AUTO-RUN
    // SlimeJump addition; requires horizontal movement.
    // Ajout pour SlimeJump ; nécessite le déplacement horizontal.
    [Header("13 | SJ EXTRA - Auto Run / Course auto")]
    [Tooltip("Adds forward movement; A slows down, D speeds up / Ajoute une course vers la droite ; A ralentit, D accélère.")]
    public bool useAutoRun = false;
    public float autoRunSpeed = 6f;

    // 14 - DEBUG
    // Shows ground detection and optional jump messages.
    // Affiche la détection du sol et les messages de saut facultatifs.
    [Header("14 | DEBUG - Visualize / Visualiser")]
    public bool drawGroundCheckGizmo = true;


    // RUNTIME VARIABLES
    // Internal values used while the game runs; not Inspector settings.
    // Valeurs internes utilisées pendant le jeu ; ne sont pas des réglages de l’Inspector.
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private float horizontalInput;
    private bool jumpHeld;
    private bool jumpPressedThisFrame;
    private bool grounded;
    private bool wasGrounded;
    private bool jumpUsedSinceGrounded;
    private bool doubleJumpUsed;
    private float lastGroundedTime = float.NegativeInfinity;
    private float lastJumpPressedTime = float.NegativeInfinity;

    public bool IsGrounded => grounded;
    public Vector2 CurrentVelocity => rb != null ? rb.linearVelocity : Vector2.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        // Gravity is calculated by this script.
        // La gravité est calculée par ce script.
        rb.gravityScale = 0f;
    }

    // INPUT
    // Reads keyboard input only: A/D or arrow keys to move, Space to jump.
    // Lit uniquement le clavier : A/D ou les flèches pour se déplacer, Espace pour sauter.
    private void Update()
    {
        horizontalInput = 0f;
        jumpHeld = false;
        if (Keyboard.current == null) return;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            horizontalInput -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            horizontalInput += 1f;

        jumpHeld = Keyboard.current.spaceKey.isPressed;
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // Stores the jump input until the next physics update.
            // Mémorise la commande de saut jusqu’à la prochaine mise à jour physique.
            jumpPressedThisFrame = true;
            lastJumpPressedTime = Time.time;
        }
    }

    // PHYSICS UPDATE
    // Updates ground detection, jumping, horizontal movement, ceiling collision and gravity.
    // Met à jour la détection du sol, le saut, le déplacement horizontal, les collisions avec le plafond et la gravité.
    private void FixedUpdate()
    {
        rb.gravityScale = 0f;
        UpdateGroundCheck();
        HandleJump();
        HandleHorizontalMovement();
        HandleCeilingCheck();
        HandleGravity();
    }

    // 05 - GROUND CHECK
    // Optional for teaching; turning it off allows jumping in midair.
    // Facultatif pour le cours ; sa désactivation permet de sauter dans les airs.
    private void UpdateGroundCheck()
    {
        wasGrounded = grounded;
        grounded = !useGroundCheck ||
                   Physics2D.OverlapCircle(GroundCheckPosition(), groundCheckRadius, groundLayer) != null;

        if (!useGroundCheck)
        {
            // Allows jumping anywhere.
            // Permet de sauter n’importe où.
            jumpUsedSinceGrounded = false;
            doubleJumpUsed = false;
        }
        else if (grounded && !wasGrounded)
        {
            // Resets the jump when the player lands.
            // Réinitialise le saut lorsque le joueur atterrit.
            jumpUsedSinceGrounded = false;
            doubleJumpUsed = false;
        }

        if (grounded && !jumpUsedSinceGrounded)
            lastGroundedTime = Time.time;
    }

    private Vector2 GroundCheckPosition()
    {
        return groundCheckPoint != null
            ? (Vector2)groundCheckPoint.position
            : (Vector2)transform.position + groundCheckOffset;
    }

    // 04 / 07 / 08 / 08B - JUMP AND DOUBLE JUMP
    // Handles jumps, coyote time, jump buffering and one extra air jump.
    // Gère les sauts, le temps de grâce, la mémoire du saut et un saut aérien supplémentaire.
    private void HandleJump()
    {
        if (!enableJump)
        {
            jumpPressedThisFrame = false;
            lastJumpPressedTime = float.NegativeInfinity;
            return;
        }

        bool buffered = useJumpBuffer &&
                        Time.time - lastJumpPressedTime <= jumpBufferTime;
        bool wantsJump = jumpPressedThisFrame || buffered;

        bool coyoteAllowed = useCoyoteTime && useGroundCheck &&
                             Time.time - lastGroundedTime <= coyoteTime;
        bool canRegularJump = !useGroundCheck ||
                              (!jumpUsedSinceGrounded && (grounded || coyoteAllowed));
        bool canDoubleJump = useDoubleJump && useGroundCheck && !grounded &&
                             jumpUsedSinceGrounded && !doubleJumpUsed;

        if (wantsJump && (canRegularJump || canDoubleJump))
        {
            Vector2 velocity = rb.linearVelocity;
            velocity.y = jumpForce;
            rb.linearVelocity = velocity;

            jumpUsedSinceGrounded = true;
            if (canDoubleJump)
                doubleJumpUsed = true;

            // Consumes the stored jump request.
            // Consomme la commande de saut mémorisée.
            lastJumpPressedTime = float.NegativeInfinity;


        }

        jumpPressedThisFrame = false;
    }

    // 01 / 02 / 03 / 11 / 13 - HORIZONTAL MOVEMENT
    // Handles acceleration, air control, apex speed bonus and auto-run.
    // Gère l’accélération, le contrôle aérien, le bonus de vitesse au sommet et la course automatique.
    private void HandleHorizontalMovement()
    {
        if (!enableHorizontalMovement) return;
        // Preserves horizontal momentum in the air.
        // Conserve l’élan horizontal dans les airs.
        if (useGroundCheck && !grounded && !useAirControl) return;

        float inputScale = grounded ? 1f : airControlMultiplier;
        float requestedSpeed = horizontalInput * maxMoveSpeed * inputScale;

        if (useAutoRun)
            requestedSpeed = Mathf.Max(0f, autoRunSpeed + requestedSpeed);

        if (useApexSpeedBonus && !grounded &&
            Mathf.Abs(rb.linearVelocity.y) <= apexSpeedThreshold &&
            Mathf.Abs(horizontalInput) > 0.01f)
        {
            requestedSpeed += Mathf.Sign(horizontalInput) * apexHorizontalBonus;
            if (useAutoRun) requestedSpeed = Mathf.Max(0f, requestedSpeed);
        }

        Vector2 velocity = rb.linearVelocity;
        if (useAcceleration)
        {
            bool hasInput = Mathf.Abs(horizontalInput) > 0.01f;
            float deceleration = !grounded && useDifferentAirDeceleration
                ? airDeceleration : groundDeceleration;
            float rate = hasInput || useAutoRun ? acceleration : deceleration;
            velocity.x = Mathf.MoveTowards(velocity.x, requestedSpeed,
                                           rate * Time.fixedDeltaTime);
        }
        else
        {
            // Applies horizontal movement immediately.
            // Applique immédiatement le déplacement horizontal.
            velocity.x = requestedSpeed;
        }

        rb.linearVelocity = velocity;
    }

    // 06 - CEILING CHECK
    // Optional; stops upward movement when the player touches a ceiling.
    // Facultatif ; arrête la montée lorsque le joueur touche un plafond.
    private void HandleCeilingCheck()
    {
        if (!useCeilingCheck || playerCollider == null || rb.linearVelocity.y <= 0f)
            return;

        Vector2 probe = new Vector2(playerCollider.bounds.center.x,
                                    playerCollider.bounds.max.y + ceilingCheckRadius * 0.4f);
        if (Physics2D.OverlapCircle(probe, ceilingCheckRadius, groundLayer) != null)
        {
            Vector2 velocity = rb.linearVelocity;
            velocity.y = 0f;
            rb.linearVelocity = velocity;
        }
    }

    // 09 / 10 / 11 / 12 - GRAVITY
    // Handles variable jump height, fast falling, apex hang and terminal velocity.
    // Gère la hauteur de saut variable, la chute rapide, la suspension au sommet et la vitesse maximale de chute.
    private void HandleGravity()
    {
        Vector2 velocity = rb.linearVelocity;

        if (useGroundCheck && grounded && velocity.y <= 0f)
        {
            if (useGroundStick) velocity.y = -Mathf.Abs(groundStickSpeed);
        }
        else if (enableGravity)
        {
            float gravity = gravityAcceleration;

            if (useVariableJumpHeight && jumpUsedSinceGrounded &&
                !jumpHeld && velocity.y > 0f)
                gravity *= Mathf.Max(1f, jumpCutGravityMultiplier);

            if (useFastFall && velocity.y < 0f)
                gravity *= Mathf.Max(1f, fastFallMultiplier);

            if (useApexHang && !grounded &&
                Mathf.Abs(velocity.y) <= apexSpeedThreshold)
                gravity *= apexGravityMultiplier;

            velocity.y -= gravity * Time.fixedDeltaTime;
        }

        if (useMaxFallSpeed)
            velocity.y = Mathf.Max(velocity.y, -Mathf.Abs(maxFallSpeed));

        rb.linearVelocity = velocity;
    }

    // 14 - DEBUG GIZMOS
    // Shows yellow probes in edit mode and green/red probes during play.
    // Affiche des détecteurs jaunes en mode édition et verts/rouges pendant le jeu.
    private void OnDrawGizmos()
    {
        if (!drawGroundCheckGizmo) return;

        Gizmos.color = Application.isPlaying
            ? (grounded ? Color.green : Color.red)
            : Color.yellow;
        Gizmos.DrawWireSphere(GroundCheckPosition(), groundCheckRadius);

        if (useCeilingCheck)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                Gizmos.color = Color.cyan;
                Vector2 probe = new Vector2(col.bounds.center.x,
                                            col.bounds.max.y + ceilingCheckRadius * 0.4f);
                Gizmos.DrawWireSphere(probe, ceilingCheckRadius);
            }
        }
    }

    // VALIDATION
    // Prevents negative or invalid Inspector values.
    // Empêche les valeurs négatives ou invalides dans l’Inspector.
    private void OnValidate()
    {
        groundCheckRadius = Mathf.Max(0.01f, groundCheckRadius);
        ceilingCheckRadius = Mathf.Max(0.01f, ceilingCheckRadius);
        maxMoveSpeed = Mathf.Max(0f, maxMoveSpeed);
        jumpForce = Mathf.Max(0f, jumpForce);
        gravityAcceleration = Mathf.Max(0f, gravityAcceleration);
        maxFallSpeed = Mathf.Max(0.01f, maxFallSpeed);
        coyoteTime = Mathf.Max(0f, coyoteTime);
        jumpBufferTime = Mathf.Max(0f, jumpBufferTime);
        apexSpeedThreshold = Mathf.Max(0f, apexSpeedThreshold);
        autoRunSpeed = Mathf.Max(0f, autoRunSpeed);
    }
}
