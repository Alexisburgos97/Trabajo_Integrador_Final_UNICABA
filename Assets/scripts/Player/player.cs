using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class TopDownCarController : MonoBehaviour
{
    [Header("Aceleración / Velocidades")]
    [SerializeField] float accelForward = 120f;
    [SerializeField] float accelReverse = 100f;
    [SerializeField] float maxSpeedForward = 50f;
    [SerializeField] float maxSpeedReverse = 50f;

    [Header("Dirección (yaw)")]
    [SerializeField] float steerAngleAt0 = 70f;   // gira MUCHO a baja velocidad
    [SerializeField] float steerAngleAtMax = 50f; // gira POCO a alta velocidad
    [SerializeField] float steerResponse = 7f;    // rapidez al tomar giro
    [SerializeField] float autoAlign = 2.5f;      // endereza según dirección de avance

    [Header("Grip / Derrape base")]
    [Range(0f,1f)] [SerializeField] float lateralGrip = 0.6f;
    [Range(0f,1f)] [SerializeField] float forwardGrip = 1f;
    [Range(0f,1f)] [SerializeField] float handbrakeLateralGrip = 0.6f;
    [SerializeField] float extraDragWhenNoThrottle = 0.8f;

    [Header("DRIFT (W/↑ + A/D + Shift)")]
    [SerializeField] float driftGrip = 1f;
    [SerializeField] float driftYawBoost = 1f;
    [SerializeField] float driftMinSpeed = 1f;
    [SerializeField] float steerDeadZone = 0.5f;
    [SerializeField] float driftHoldSeconds = 0.5f;

    [Header("SALTO")]
    [SerializeField] KeyCode jumpKey = KeyCode.E;
    [SerializeField] float jumpVelocity = 60f;     // VelocityChange
    [SerializeField] float jumpCooldown = 0f;
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] float jumpBuffer = 0.1f;
    [SerializeField] float extraGravity = 200f;
    [SerializeField] float jumpCutGravity = 0f;

    [Header("Detección de suelo / rampas")]
    [SerializeField] float groundCheckDistance = 1f;
    [SerializeField] float groundProbeRadius = 0f;
    [SerializeField] LayerMask groundMask = ~0;

    [Header("Control en el aire")]
    [SerializeField] float airSteerMultiplier = 0.45f;
    [SerializeField] float airAccelMultiplier = 0.65f;

    [Header("Frenos")]
    [SerializeField] float brakeStrength = 250f;
    [SerializeField] KeyCode brakeKey = KeyCode.LeftControl;
    [SerializeField] KeyCode handbrakeKey = KeyCode.Space;

    [Header("Rigidbody")]
    [SerializeField] float baseDrag = 0.15f;
    [SerializeField] Vector3 centerOfMassOffset = new Vector3(0,-0.35f,0);

    [Header("Colisión externa (lock)")]
    [SerializeField] float defaultLockSeconds = 0.25f;
    [SerializeField] float defaultExtraBrake = 120f;

    [Header("EVENTOS (VFX/SFX)")]
    public UnityEvent onJump;   // se dispara al despegar
    public UnityEvent onLand;   // se dispara al aterrizar

    // --- Privados ---
    Rigidbody rb;
    float steerInput, throttleInput;
    bool braking, handbrake;

    // ambiente
    float envSpeedMult = 1f, envAccelMult = 1f, envGripMult = 1f, envExtraDrag = 0f;
    bool envActive = false;

    // lock externo
    float externalLockTimer = 0f;
    float externalExtraBrake = 0f;

    // drift
    bool isDrifting = false;
    float driftTimer = 0f;
    public bool IsDrifting => isDrifting;

    // salto / suelo
    bool grounded = false;
    bool wasGrounded = true;          // para detectar transición aire->suelo
    Vector3 groundNormal = Vector3.up;
    float coyoteTimer = 0f;
    float jumpBufferTimer = 0f;
    float nextJumpTime = 0f;
    bool jumpHeld = false;
    float lastYVel = 0f;              // velocidad vertical del frame anterior
    [SerializeField] float landMinSpeed = -6f; // si cae más rápido que esto => onLand

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.linearDamping = baseDrag;
        rb.angularDamping = 0.05f;
        rb.centerOfMass += centerOfMassOffset;
    }

    void Update()
    {
        if (externalLockTimer > 0f)
        {
            throttleInput = 0f;
            steerInput = Mathf.MoveTowards(steerInput, 0f, 10f * Time.deltaTime);
            braking = true;
            handbrake = false;
        }
        else
        {
            throttleInput = Input.GetAxisRaw("Vertical");
            steerInput    = Input.GetAxisRaw("Horizontal");
            braking       = Input.GetKey(brakeKey);
            handbrake     = Input.GetKey(handbrakeKey) || Input.GetKey(KeyCode.RightShift);
        }

        if (Input.GetKeyDown(jumpKey)) jumpBufferTimer = jumpBuffer;
        jumpHeld = Input.GetKey(jumpKey);

        if (jumpBufferTimer > 0f) jumpBufferTimer -= Time.deltaTime;
        if (coyoteTimer > 0f) coyoteTimer -= Time.deltaTime;
        if (externalLockTimer > 0f)
        {
            externalLockTimer -= Time.deltaTime;
            if (externalLockTimer <= 0f) externalExtraBrake = 0f;
        }

        lastYVel = rb.linearVelocity.y; // para el evento onLand
    }

    void FixedUpdate()
    {
        // ====== GROUND CHECK ======
        groundNormal = Vector3.up;
        grounded = false;

        Vector3 origin = transform.position + Vector3.up * 0.2f;
        if (Physics.SphereCast(origin, groundProbeRadius, Vector3.down, out var hit,
            groundCheckDistance + groundProbeRadius, groundMask, QueryTriggerInteraction.Ignore))
        {
            grounded = hit.distance <= (groundCheckDistance + groundProbeRadius);
            groundNormal = hit.normal;
        }
        if (grounded) coyoteTimer = coyoteTime;

        // --- Detectar aterrizaje (aire -> suelo)
        if (grounded && !wasGrounded)
        {
            if (lastYVel <= landMinSpeed)
                onLand?.Invoke();
        }
        wasGrounded = grounded;

        // ====== Estado DRIFT (combo) ======
        Vector3 vel = rb.linearVelocity;
        Vector3 velXZ = new Vector3(vel.x, 0f, vel.z);
        float speed = velXZ.magnitude;

        bool driftCombo = handbrake && throttleInput > 0.2f && Mathf.Abs(steerInput) > steerDeadZone && speed > driftMinSpeed && grounded;
        if (driftCombo) driftTimer = driftHoldSeconds;
        else if (driftTimer > 0f) driftTimer -= Time.fixedDeltaTime;
        isDrifting = driftTimer > 0f;

        // ====== Aceleración ======
        float accel = (throttleInput >= 0 ? accelForward : accelReverse) * (grounded ? 1f : airAccelMultiplier) * envAccelMult;
        Vector3 moveDir = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized;
        rb.AddForce(moveDir * throttleInput * accel, ForceMode.Acceleration);

        // ====== Clamp de velocidad ======
        float baseMax = (throttleInput >= 0 ? maxSpeedForward : maxSpeedReverse) * envSpeedMult;
        Vector3 dirSpeed = Vector3.Project(rb.linearVelocity, moveDir);
        Vector3 sideSpeed = rb.linearVelocity - dirSpeed;
        if (dirSpeed.magnitude > baseMax)
            dirSpeed = dirSpeed.normalized * baseMax;

        // ====== Grip ======
        float currentLateralGrip = isDrifting ? driftGrip : (handbrake ? handbrakeLateralGrip : lateralGrip);
        currentLateralGrip *= envGripMult;
        sideSpeed *= currentLateralGrip;
        dirSpeed  *= forwardGrip;
        rb.linearVelocity = dirSpeed + sideSpeed;

        // ====== Dirección (yaw) y alineación a pendiente ======
        float speed01 = Mathf.InverseLerp(0f, maxSpeedForward, speed);
        float targetSteer = Mathf.Lerp(steerAngleAt0, steerAngleAtMax, speed01) * steerInput;
        float yawMult = (isDrifting ? driftYawBoost : 1f) * (grounded ? 1f : airSteerMultiplier);

        Quaternion yawDelta = Quaternion.AngleAxis(targetSteer * yawMult * Time.fixedDeltaTime * steerResponse,
                                                   grounded ? groundNormal : Vector3.up);

        float slopeAlignSpeed = 10f;
        Quaternion alignToGround = Quaternion.FromToRotation(transform.up, groundNormal);
        Quaternion targetRot = alignToGround * rb.rotation * yawDelta;
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, slopeAlignSpeed * Time.fixedDeltaTime));

        // ====== Salto ======
        TryJump();

        // ====== Gravedad extra / jump cut ======
        if (!grounded)
        {
            rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
            if (!jumpHeld && rb.linearVelocity.y > 0f)
                rb.AddForce(Vector3.down * jumpCutGravity, ForceMode.Acceleration);
        }

        // ====== Frenos ======
        if (Input.GetKey(brakeKey))
            rb.AddForce(-rb.linearVelocity.normalized * brakeStrength, ForceMode.Acceleration);
        else if (Mathf.Approximately(throttleInput, 0f))
            rb.linearVelocity *= (1f - (extraDragWhenNoThrottle * Time.fixedDeltaTime));

        // ====== Lock externo ======
        if (externalLockTimer > 0f)
            rb.AddForce(-rb.linearVelocity.normalized * (brakeStrength + externalExtraBrake), ForceMode.Acceleration);

        // ====== Drag por ambiente ======
        rb.linearDamping = baseDrag + (envActive ? envExtraDrag : 0f);
    }

    void TryJump()
    {
        if (Time.time < nextJumpTime) return;
        if (jumpBufferTimer <= 0f || coyoteTimer <= 0f) return;

        // limpiar vertical si venías cayendo
        Vector3 v = rb.linearVelocity; v.y = Mathf.Max(0f, v.y); rb.linearVelocity = v;

        // impulso vertical independiente de masa
        rb.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);

        // >>> evento
        onJump?.Invoke();

        jumpBufferTimer = 0f;
        coyoteTimer = 0f;
        nextJumpTime = Time.time + jumpCooldown;
    }

    // === API de entorno ===
    public void ApplyEnvironment(float speedMult, float accelMult, float gripMult, float extraDrag, bool overrideIfActive = false)
    {
        if (envActive && !overrideIfActive) return;
        envSpeedMult = Mathf.Clamp(speedMult, 0.1f, 1f);
        envAccelMult = Mathf.Clamp(accelMult, 0.1f, 1f);
        envGripMult  = Mathf.Clamp(gripMult,  0.1f, 1f);
        envExtraDrag = Mathf.Max(0f, extraDrag);
        envActive    = true;
    }
    public void ClearEnvironment()
    {
        envSpeedMult = envAccelMult = envGripMult = 1f;
        envExtraDrag = 0f;
        envActive    = false;
    }

    // === Compatibilidad con Detect_Pared ===
    public void Frenar() => Frenar(defaultLockSeconds, defaultExtraBrake);
    public void Frenar(float lockSeconds, float extraBrake)
    {
        externalLockTimer = Mathf.Max(externalLockTimer, lockSeconds);
        externalExtraBrake = Mathf.Max(externalExtraBrake, extraBrake);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        Gizmos.DrawWireSphere(origin + Vector3.down * groundCheckDistance, groundProbeRadius);
    }
#endif
}
