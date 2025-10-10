/*using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    [Header("Velocidades")]
    [SerializeField] float _moveSpeed = 10f;      // Aceleración hacia adelante/atrás
    [SerializeField] float _maxSpeed = 20f;       // Velocidad máxima
    [SerializeField] float _turnSpeed = 100f;     // Velocidad de giro
    [SerializeField] float _brakeForce = 30f;     // Fuerza de frenado
    [SerializeField] float _handbrakeDrag = 2f;   // Cuánto se frena al tirar el freno de mano
    [SerializeField] float _driftFactor = 0.95f;  // Factor de derrape (más bajo = más drift)
    [Header("Linear damping")]
    [SerializeField] float _drag = 1; //valor pordefecto linear damping

    private Rigidbody _rb;

    private float _moveInput;
    private float _turnInput;
    private bool _isBraking;
    private bool _isHandbrake;
    private bool _frenado = false;   // <-- Nuevo flag

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        if (_frenado) return; // Si está frenado, ignoramos input

        _moveInput = Input.GetAxis("Vertical");
        _turnInput = Input.GetAxis("Horizontal");

        // Barra espaciadora = freno normal
        _isBraking = Input.GetKey(KeyCode.Space);

        // Shift = freno de mano
        _isHandbrake = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }
    /*
    void FixedUpdate()
    {
        if (_frenado) return; // Si está frenado, no aplicar movimiento

        // Movimiento adelante / atrás
        Vector3 move = transform.forward * _moveInput * _moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + move);

        // Rotación izquierda / derecha
        float turn = _turnInput * _turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        _rb.MoveRotation(_rb.rotation * turnRotation);
    }
    #1#    
        
    void FixedUpdate()
    {
        // Movimiento normal
        _rb.AddForce(transform.forward * _moveInput * _moveSpeed, ForceMode.Acceleration);

        // Limitar velocidad máxima en plano XZ
        Vector3 vel = _rb.linearVelocity;
        Vector3 velXZ = new Vector3(vel.x, 0f, vel.z);
        if (velXZ.magnitude > _maxSpeed)
        {
            velXZ = velXZ.normalized * _maxSpeed;
            _rb.linearVelocity = new Vector3(velXZ.x, vel.y, velXZ.z);
        }

        // Rotación del coche
        float turn = _turnInput * _turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        _rb.MoveRotation(_rb.rotation * turnRotation);

        // --- Freno normal ---
        if (_isBraking)
        {
            _rb.linearVelocity *= 0.9f; // Desaceleración suave
            _rb.AddForce(-_rb.linearVelocity.normalized * _brakeForce, ForceMode.Acceleration);
        }

        // --- Freno de mano / Drift ---
        if (_isHandbrake)
        {
            // Reducir la fricción lateral para que "patine"
            Vector3 localVel = transform.InverseTransformDirection(_rb.linearVelocity);
            localVel.x *= _driftFactor; // Eje lateral = derrape
            _rb.linearVelocity = transform.TransformDirection(localVel);

            // Añadimos un drag extra para que no se acelere infinito
            _rb.linearDamping = _handbrakeDrag;
        }
        else
        {
            _rb.linearDamping = _drag; // Valor normal (ajustalo para que no se sienta hielo)
        }
    }

    // Método para detener el coche
    
    public void Frenar()
    {
        _moveInput = 0;
        _turnInput = 0;
        _frenado = true;
    }

    // Si querés que se pueda volver a mover después
    public void Reanudar()
    {
        _frenado = false;
    }
  
}*/

// using UnityEngine;
//
// [RequireComponent(typeof(Rigidbody))]
// public class TopDownCarController : MonoBehaviour
// {
//     [Header("Aceleración / Velocidades")]
//     [SerializeField] float accelForward = 120f;
//     [SerializeField] float accelReverse = 100f;
//     [SerializeField] float maxSpeedForward = 50f;
//     [SerializeField] float maxSpeedReverse = 50f;
//
//     [Header("Dirección")]
//     [SerializeField] float steerAngleAt0 = 20f;    // gira MUCHO a baja velocidad
//     [SerializeField] float steerAngleAtMax = 18f;  // gira POCO a alta velocidad
//     [SerializeField] float steerResponse = 7f;     // rapidez con la que “toma” el giro
//     [SerializeField] float autoAlign = 2.5f;       // endereza el auto con la velocidad
//
//     [Header("Grip / Drift")]
//     [Range(0f,1f)] [SerializeField] float lateralGrip = 0.6f; // 1 = sin derrape, 0 = hielo
//     [Range(0f,1f)] [SerializeField] float forwardGrip = 1f;
//     [Range(0f,1f)] [SerializeField] float handbrakeLateralGrip = 0.6f; // grip lateral cuando tirás freno de mano
//     [SerializeField] float extraDragWhenNoThrottle = 0.8f; // frena un toque si soltás el acelerador
//
//     [Header("DRIFT (combo: adelante + izquierda/derecha + Shift)")]
//     [SerializeField] float driftGrip = 0.95f;      // ↓ grip lateral durante el drift
//     [SerializeField] float driftYawBoost = 1f;   // multiplica el giro mientras derrapa
//     [SerializeField] float driftMinSpeed = 1f;     // velocidad mínima para permitir drift
//     [SerializeField] float steerDeadZone = 0.5f;   // cuánto hay que girar para activar
//     [SerializeField] float driftHoldSeconds = 0.5f; // cuánto “se sostiene” tras soltar
//     
//     [Header("Frenos")]
//     [SerializeField] float brakeStrength = 250f;
//     [SerializeField] KeyCode brakeKey = KeyCode.Space;
//     [SerializeField] KeyCode handbrakeKey = KeyCode.LeftShift;
//
//     [Header("Rigidbody")]
//     [SerializeField] float baseDrag = 0.15f;
//     [SerializeField] Vector3 centerOfMassOffset = new Vector3(0,-0.35f,0);
//     
//     // —— Reacción externa (colisiones/paredes) ——
//     [Header("Colisión externa")]
//     [SerializeField] float defaultLockSeconds = 0.25f;
//     [SerializeField] float defaultExtraBrake = 120f;
//
//     Rigidbody rb;
//     float steerInput, throttleInput;
//     bool braking, handbrake;
//     
//     // timers/estados externos
//     float externalLockTimer = 0f;
//     float externalExtraBrake = 0f;
//     
//     // estado de drift
//     bool isDrifting = false;
//     float driftTimer = 0f;
//     
//     public bool IsDrifting => isDrifting; // por si querés activar VFX/HUD
//
//     void Awake()
//     {
//         rb = GetComponent<Rigidbody>();
//         rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
//         rb.interpolation = RigidbodyInterpolation.Interpolate;
//         rb.linearDamping = baseDrag;
//         rb.angularDamping = 0.05f;
//         rb.centerOfMass += centerOfMassOffset;
//     }
//
//     void Update()
//     {
//         if (externalLockTimer > 0f)
//         {
//             // bloqueo suave del input
//             throttleInput = 0f;
//             steerInput = Mathf.MoveTowards(steerInput, 0f, 10f * Time.deltaTime);
//             braking = true;
//             handbrake = false;
//         }
//         else
//         {
//             throttleInput = Input.GetAxisRaw("Vertical");
//             steerInput    = Input.GetAxisRaw("Horizontal");
//             braking       = Input.GetKey(brakeKey);
//             handbrake     = Input.GetKey(handbrakeKey) || Input.GetKey(KeyCode.RightShift);
//         }
//     }
//
//     void FixedUpdate()
//     {
//         Vector3 vel = rb.linearVelocity;
//         Vector3 fwd = transform.forward;
//
//         // Velocidad en XZ
//         Vector3 velXZ = new Vector3(vel.x, 0, vel.z);
//         float speed = velXZ.magnitude;
//         
//         // 0) Estado de DRIFT (combo)
//         bool driftCombo = handbrake && throttleInput > 0.2f && Mathf.Abs(steerInput) > steerDeadZone && speed > driftMinSpeed;
//         if (driftCombo) driftTimer = driftHoldSeconds;
//         else if (driftTimer > 0f) driftTimer -= Time.fixedDeltaTime;
//
//         isDrifting = driftTimer > 0f;
//
//         // 1) Aceleración
//         float accel = throttleInput >= 0 ? accelForward : accelReverse;
//         rb.AddForce(fwd * throttleInput * accel, ForceMode.Acceleration);
//
//         // 2) Clamp de velocidad
//         float maxSpeed = throttleInput >= 0 ? maxSpeedForward : maxSpeedReverse;
//         Vector3 dirSpeed = Vector3.Project(rb.linearVelocity, fwd);
//         Vector3 sideSpeed = rb.linearVelocity - dirSpeed;
//
//         if (dirSpeed.magnitude > maxSpeed)
//             dirSpeed = dirSpeed.normalized * maxSpeed;
//
//         // 3) Grip
//         // float currentLateralGrip = handbrake ? handbrakeLateralGrip : lateralGrip;
//         // sideSpeed *= currentLateralGrip;
//         // dirSpeed  *= forwardGrip;
//         // rb.linearVelocity = dirSpeed + sideSpeed;
//         
//         // 3) Grip (normal / handbrake / drift)
//         float currentLateralGrip = isDrifting ? driftGrip : (handbrake ? handbrakeLateralGrip : lateralGrip);
//         sideSpeed *= currentLateralGrip;
//         dirSpeed  *= forwardGrip;
//         rb.linearVelocity = dirSpeed + sideSpeed;
//
//         // 4) Dirección dependiente de velocidad
//         float speed01 = Mathf.InverseLerp(0f, maxSpeedForward, speed);
//         float targetSteer = Mathf.Lerp(steerAngleAt0, steerAngleAtMax, speed01) * steerInput;
//         Quaternion steerRot = Quaternion.AngleAxis(targetSteer * Time.fixedDeltaTime * steerResponse, Vector3.up);
//         rb.MoveRotation(rb.rotation * steerRot);
//
//         // 5) Auto-align
//         if (speed > 0.5f)
//         {
//             Vector3 heading = Vector3.Slerp(fwd, velXZ.normalized, autoAlign * Time.fixedDeltaTime);
//             Quaternion alignRot = Quaternion.LookRotation(heading, Vector3.up);
//             rb.MoveRotation(Quaternion.Slerp(rb.rotation, alignRot, 0.35f * Time.fixedDeltaTime));
//         }
//
//         // 6) Frenos
//         if (braking)
//             rb.AddForce(-rb.linearVelocity.normalized * brakeStrength, ForceMode.Acceleration);
//         else if (Mathf.Approximately(throttleInput, 0f))
//             rb.linearVelocity *= (1f - (extraDragWhenNoThrottle * Time.fixedDeltaTime));
//
//         // 7) Frenado externo mientras dure el lock
//         if (externalLockTimer > 0f)
//         {
//             rb.AddForce(-rb.linearVelocity.normalized * (brakeStrength + externalExtraBrake), ForceMode.Acceleration);
//             externalLockTimer -= Time.fixedDeltaTime;
//             if (externalLockTimer <= 0f) externalExtraBrake = 0f;
//         }
//     }
//     
//     // —— Compatibilidad con tu Detect_Pared antiguo ——
//     public void Frenar() => Frenar(defaultLockSeconds, defaultExtraBrake);
//
//     // Overload para customizar
//     public void Frenar(float lockSeconds, float extraBrake)
//     {
//         externalLockTimer = Mathf.Max(externalLockTimer, lockSeconds);
//         externalExtraBrake = Mathf.Max(externalExtraBrake, extraBrake);
//     }
// }





/*using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TopDownCarController : MonoBehaviour
{
    [Header("Aceleración / Velocidades")]
    [SerializeField] float accelForward = 120f;
    [SerializeField] float accelReverse = 100f;
    [SerializeField] float maxSpeedForward = 50f;
    [SerializeField] float maxSpeedReverse = 50f;

    [Header("Dirección")]
    [SerializeField] float steerAngleAt0 = 70f;
    [SerializeField] float steerAngleAtMax = 18f;
    [SerializeField] float steerResponse = 7f;
    [SerializeField] float autoAlign = 2.5f;

    [Header("Grip / Derrape base")]
    [Range(0f,1f)] [SerializeField] float lateralGrip = 0.6f;
    [Range(0f,1f)] [SerializeField] float forwardGrip = 1f;
    [Range(0f,1f)] [SerializeField] float handbrakeLateralGrip = 0.6f;
    [SerializeField] float extraDragWhenNoThrottle = 0.8f;

    [Header("DRIFT (adelante + izq/der + Shift)")]
    [SerializeField] float driftGrip = 0.45f;
    [SerializeField] float driftYawBoost = 2.1f;
    [SerializeField] float driftMinSpeed = 6f;
    [SerializeField] float steerDeadZone = 0.2f;
    [SerializeField] float driftHoldSeconds = 0.20f;

    [Header("SALTO")]
    [SerializeField] KeyCode jumpKey = KeyCode.E;     // tecla de salto
    [SerializeField] float jumpVelocity = 7.5f;       // m/s verticales (VelocityChange)
    [SerializeField] float jumpCooldown = 0.15f;
    [SerializeField] float coyoteTime = 0.12f;
    [SerializeField] float jumpBuffer = 0.12f;
    [SerializeField] float extraGravity = 20f;        // m/s^2 extra hacia abajo
    [SerializeField] float jumpCutGravity = 30f;      // si soltás la tecla subiendo
    [SerializeField] float groundCheckDistance = 0.7f;
    [SerializeField] LayerMask groundMask = ~0;       // asigná Ground acá

    [Header("Control en el aire")]
    [SerializeField] float airSteerMultiplier = 0.45f;
    [SerializeField] float airAccelMultiplier = 0.65f;

    [Header("Frenos")]
    [SerializeField] float brakeStrength = 250f;
    [SerializeField] KeyCode brakeKey = KeyCode.Space;
    [SerializeField] KeyCode handbrakeKey = KeyCode.LeftShift;

    [Header("Rigidbody")]
    [SerializeField] float baseDrag = 0.15f;
    [SerializeField] Vector3 centerOfMassOffset = new Vector3(0,-0.35f,0);

    [Header("Colisión externa")]
    [SerializeField] float defaultLockSeconds = 0.25f;
    [SerializeField] float defaultExtraBrake = 120f;

    Rigidbody rb;
    float steerInput, throttleInput;
    bool braking, handbrake;

    // externos
    float externalLockTimer = 0f;
    float externalExtraBrake = 0f;

    // drift
    bool isDrifting = false;
    float driftTimer = 0f;
    public bool IsDrifting => isDrifting;

    // salto
    bool grounded = false;
    float coyoteTimer = 0f;
    float jumpBufferTimer = 0f;
    float nextJumpTime = 0f;
    bool jumpHeld = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.linearDamping = baseDrag;
        rb.angularDamping = 0.05f;
        rb.centerOfMass += centerOfMassOffset;
    }

    void Update()
    {
        // INPUT base (con lock externo)
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

        // INPUT salto (buffer + held)
        if (Input.GetKeyDown(jumpKey)) jumpBufferTimer = jumpBuffer;
        jumpHeld = Input.GetKey(jumpKey);

        if (jumpBufferTimer > 0f) jumpBufferTimer -= Time.deltaTime;
        if (coyoteTimer > 0f) coyoteTimer -= Time.deltaTime;
        if (externalLockTimer > 0f) externalLockTimer -= Time.deltaTime;
        if (externalLockTimer <= 0f) externalExtraBrake = 0f;
    }

    void FixedUpdate()
    {
        // ---- GROUND CHECK ----
        grounded = Physics.Raycast(transform.position + Vector3.up * 0.1f,
                                   Vector3.down, out var hit,
                                   groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore);
        if (grounded) coyoteTimer = coyoteTime;

        Vector3 vel = rb.linearVelocity;
        Vector3 fwd = transform.forward;
        Vector3 velXZ = new Vector3(vel.x, 0, vel.z);
        float speed = velXZ.magnitude;

        // ---- DRIFT state (adelante + steer + shift + velocidad) ----
        bool driftCombo = handbrake && throttleInput > 0.2f && Mathf.Abs(steerInput) > steerDeadZone && speed > driftMinSpeed && grounded;
        if (driftCombo) driftTimer = driftHoldSeconds;
        else if (driftTimer > 0f) driftTimer -= Time.fixedDeltaTime;
        isDrifting = driftTimer > 0f;

        // ---- ACELERACIÓN (menos en el aire) ----
        float accel = throttleInput >= 0 ? accelForward : accelReverse;
        if (!grounded) accel *= airAccelMultiplier;
        rb.AddForce(fwd * throttleInput * accel, ForceMode.Acceleration);

        // ---- CLAMP VELOCIDAD ----
        float maxSpeed = throttleInput >= 0 ? maxSpeedForward : maxSpeedReverse;
        Vector3 dirSpeed = Vector3.Project(rb.linearVelocity, fwd);
        Vector3 sideSpeed = rb.linearVelocity - dirSpeed;
        if (dirSpeed.magnitude > maxSpeed) dirSpeed = dirSpeed.normalized * maxSpeed;

        // ---- GRIP ----
        float currentLateralGrip = isDrifting ? driftGrip : (handbrake ? handbrakeLateralGrip : lateralGrip);
        sideSpeed *= currentLateralGrip;
        dirSpeed  *= forwardGrip;
        rb.linearVelocity = dirSpeed + sideSpeed;

        // ---- DIRECCIÓN (menos en el aire; más yaw en drift) ----
        float speed01 = Mathf.InverseLerp(0f, maxSpeedForward, speed);
        float targetSteer = Mathf.Lerp(steerAngleAt0, steerAngleAtMax, speed01) * steerInput;
        float yawMult = (isDrifting ? driftYawBoost : 1f) * (grounded ? 1f : airSteerMultiplier);
        Quaternion steerRot = Quaternion.AngleAxis(targetSteer * yawMult * Time.fixedDeltaTime * steerResponse, Vector3.up);
        rb.MoveRotation(rb.rotation * steerRot);

        // ---- AUTO-ALIGN (casi nulo en aire y reducido en drift) ----
        float alignFactor =
            grounded ? (isDrifting ? (autoAlign * 0.45f) : autoAlign)
                     : 0f;
        if (speed > 0.5f && alignFactor > 0f)
        {
            Vector3 heading = Vector3.Slerp(transform.forward, velXZ.normalized, alignFactor * Time.fixedDeltaTime);
            Quaternion alignRot = Quaternion.LookRotation(heading, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, alignRot, 0.35f * Time.fixedDeltaTime));
        }

        // ---- SALTO ----
        TryJump();

        // ---- GRAVEDAD EXTRA / JUMP CUT ----
        if (!grounded)
        {
            // gravedad extra (más contundente)
            rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);

            // jump cut: si soltás la tecla mientras subís, caé antes (brinca corto)
            if (!jumpHeld && rb.linearVelocity.y > 0f)
                rb.AddForce(Vector3.down * jumpCutGravity, ForceMode.Acceleration);
        }

        // ---- FRENOS ----
        if (braking)
            rb.AddForce(-rb.linearVelocity.normalized * brakeStrength, ForceMode.Acceleration);
        else if (Mathf.Approximately(throttleInput, 0f))
            rb.linearVelocity *= (1f - (extraDragWhenNoThrottle * Time.fixedDeltaTime));

        // ---- LOCK externo (colisiones) ----
        if (externalLockTimer > 0f)
            rb.AddForce(-rb.linearVelocity.normalized * (brakeStrength + externalExtraBrake), ForceMode.Acceleration);
    }

    void TryJump()
    {
        if (Time.time < nextJumpTime) return;

        // jump válido si hay buffer y coyote
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            // limpiar vertical previa si venías cayendo
            Vector3 v = rb.linearVelocity;
            v.y = Mathf.Max(0f, v.y);
            rb.linearVelocity = v;

            // impulso vertical independiente de masa
            rb.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);

            // reset timers
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            nextJumpTime = Time.time + jumpCooldown;
        }
    }

    // —— Para Detect_Pared ——
    public void Frenar() => Frenar(defaultLockSeconds, defaultExtraBrake);
    public void Frenar(float lockSeconds, float extraBrake)
    {
        externalLockTimer = Mathf.Max(externalLockTimer, lockSeconds);
        externalExtraBrake = Mathf.Max(externalExtraBrake, extraBrake);
    }
    
    // Dentro de TopDownCarController
    float envSpeedMult = 1f, envAccelMult = 1f, envGripMult = 1f, envExtraDrag = 0f;
    bool  envActive = false;

    public void ApplyEnvironment(float speedMult, float accelMult, float gripMult, float extraDrag, bool overrideIfActive = false)
    {
        // si ya había otra zona, solo sobreescribí si lo pedís
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
}*/


using UnityEngine;

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
    [SerializeField] float driftGrip = 1f;     // grip lateral durante drift
    [SerializeField] float driftYawBoost = 1f;  // multiplica giro en drift
    [SerializeField] float driftMinSpeed = 1f;    // velocidad mínima para entrar
    [SerializeField] float steerDeadZone = 0.5f;  // umbral de giro para entrar
    [SerializeField] float driftHoldSeconds = 0.5f;

    [Header("SALTO")]
    [SerializeField] KeyCode jumpKey = KeyCode.E;
    [SerializeField] float jumpVelocity = 60f;   // impulso vertical (VelocityChange)
    [SerializeField] float jumpCooldown = 0;
    [SerializeField] float coyoteTime = 0.1f;    // perdón post-suelo
    [SerializeField] float jumpBuffer = 0.1f;    // perdón pre-suelo
    [SerializeField] float extraGravity = 200f;    // gravedad adicional
    [SerializeField] float jumpCutGravity = 0;  // si soltás la tecla en subida

    [Header("Detección de suelo / rampas")]
    [SerializeField] float groundCheckDistance = 1f;
    [SerializeField] float groundProbeRadius = 0;  // ayuda en bordes
    [SerializeField] LayerMask groundMask = ~0;

    [Header("Control en el aire")]
    [SerializeField] float airSteerMultiplier = 0.45f;
    [SerializeField] float airAccelMultiplier = 0.65f;

    [Header("Frenos")]
    [SerializeField] float brakeStrength = 250f;
    [SerializeField] KeyCode brakeKey = KeyCode.Space;
    [SerializeField] KeyCode handbrakeKey = KeyCode.LeftShift;

    [Header("Rigidbody")]
    [SerializeField] float baseDrag = 0.15f;
    [SerializeField] Vector3 centerOfMassOffset = new Vector3(0,-0.35f,0);

    [Header("Colisión externa (lock)")]
    [SerializeField] float defaultLockSeconds = 0.25f;
    [SerializeField] float defaultExtraBrake = 120f;

    Rigidbody rb;
    float steerInput, throttleInput;
    bool braking, handbrake;

    // ambiente (arena movediza, etc.)
    float envSpeedMult = 1f, envAccelMult = 1f, envGripMult = 1f, envExtraDrag = 0f;
    bool envActive = false;

    // lock externo
    float externalLockTimer = 0f;
    float externalExtraBrake = 0f;

    // drift
    bool isDrifting = false;
    float driftTimer = 0f;
    public bool IsDrifting => isDrifting;

    // salto
    bool grounded = false;
    Vector3 groundNormal = Vector3.up;
    float coyoteTimer = 0f;
    float jumpBufferTimer = 0f;
    float nextJumpTime = 0f;
    bool jumpHeld = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.linearDamping = baseDrag;
        rb.angularDamping = 0.05f;
        rb.centerOfMass += centerOfMassOffset;
        // IMPORTANTE: en el Inspector, NO congeles Rotation X (permití pitch para rampas)
        // podés congelar Z si querés evitar roll lateral en top-down.
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
    }

    void FixedUpdate()
    {
        // ====== GROUND CHECK con SphereCast (más estable en bordes de rampa) ======
        groundNormal = Vector3.up;
        grounded = false;

        Vector3 origin = transform.position + Vector3.up * 0.2f;
        if (Physics.SphereCast(origin, groundProbeRadius, Vector3.down, out var hit, groundCheckDistance + groundProbeRadius, groundMask, QueryTriggerInteraction.Ignore))
        {
            grounded = hit.distance <= (groundCheckDistance + groundProbeRadius);
            groundNormal = hit.normal;
        }
        if (grounded) coyoteTimer = coyoteTime;

        // ====== Estado DRIFT (combo) ======
        Vector3 vel = rb.linearVelocity;
        Vector3 velXZ = new Vector3(vel.x, 0f, vel.z);
        float speed = velXZ.magnitude;

        bool driftCombo = handbrake && throttleInput > 0.2f && Mathf.Abs(steerInput) > steerDeadZone && speed > driftMinSpeed && grounded;
        if (driftCombo) driftTimer = driftHoldSeconds; else if (driftTimer > 0f) driftTimer -= Time.fixedDeltaTime;
        isDrifting = driftTimer > 0f;

        // ====== Aceleración (proyectada al plano de la rampa) ======
        float accel = (throttleInput >= 0 ? accelForward : accelReverse) * (grounded ? 1f : airAccelMultiplier) * envAccelMult;
        Vector3 moveDir = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized;
        rb.AddForce(moveDir * throttleInput * accel, ForceMode.Acceleration);

        // ====== Clamp de velocidad ======
        float baseMax = (throttleInput >= 0 ? maxSpeedForward : maxSpeedReverse) * envSpeedMult;
        Vector3 dirSpeed = Vector3.Project(rb.linearVelocity, moveDir); // en dirección de avance sobre el plano
        Vector3 sideSpeed = rb.linearVelocity - dirSpeed;

        if (dirSpeed.magnitude > baseMax)
            dirSpeed = dirSpeed.normalized * baseMax;

        // ====== Grip ======
        float currentLateralGrip = isDrifting ? driftGrip : (handbrake ? handbrakeLateralGrip : lateralGrip);
        currentLateralGrip *= envGripMult;

        sideSpeed *= currentLateralGrip;
        dirSpeed  *= forwardGrip;
        rb.linearVelocity = dirSpeed + sideSpeed;

        // ====== Dirección (yaw) sobre la normal del suelo + alineación a pendiente ======
        float speed01 = Mathf.InverseLerp(0f, maxSpeedForward, speed);
        float targetSteer = Mathf.Lerp(steerAngleAt0, steerAngleAtMax, speed01) * steerInput;
        float yawMult = (isDrifting ? driftYawBoost : 1f) * (grounded ? 1f : airSteerMultiplier);

        // giro alrededor de la normal del suelo (no siempre Vector3.up)
        Quaternion yawDelta = Quaternion.AngleAxis(targetSteer * yawMult * Time.fixedDeltaTime * steerResponse,
                                                   grounded ? groundNormal : Vector3.up);

        // alinear el up del auto con la normal del suelo suavemente
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

        // ====== Lock externo (colisiones) ======
        if (externalLockTimer > 0f)
            rb.AddForce(-rb.linearVelocity.normalized * (brakeStrength + externalExtraBrake), ForceMode.Acceleration);

        // ====== Drag por ambiente (arena, etc.) ======
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

        jumpBufferTimer = 0f;
        coyoteTimer = 0f;
        nextJumpTime = Time.time + jumpCooldown;
    }

    // === API para zonas de entorno (arena movediza, hielo, etc.) ===
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

    // === Compatibilidad con tu Detect_Pared ===
    public void Frenar() => Frenar(defaultLockSeconds, defaultExtraBrake);
    public void Frenar(float lockSeconds, float extraBrake)
    {
        externalLockTimer = Mathf.Max(externalLockTimer, lockSeconds);
        externalExtraBrake = Mathf.Max(externalExtraBrake, extraBrake);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // debug del ground check
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        Gizmos.DrawWireSphere(origin + Vector3.down * groundCheckDistance, groundProbeRadius);
    }
#endif
}