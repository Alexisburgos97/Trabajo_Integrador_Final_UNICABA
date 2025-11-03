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

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TopDownCarController : MonoBehaviour
{
    [Header("Aceleración / Velocidades")]
    [SerializeField] float accelForward = 120f;
    [SerializeField] float accelReverse = 100f;
    [SerializeField] float maxSpeedForward = 50f;
    [SerializeField] float maxSpeedReverse = 50f;

    [Header("Dirección")]
    [SerializeField] float steerAngleAt0 = 70f;    // gira MUCHO a baja velocidad
    [SerializeField] float steerAngleAtMax = 18f;  // gira POCO a alta velocidad
    [SerializeField] float steerResponse = 7f;     // rapidez con la que “toma” el giro
    [SerializeField] float autoAlign = 2.5f;       // endereza el auto con la velocidad

    [Header("Grip / Drift")]
    [Range(0f,1f)] [SerializeField] float lateralGrip = 0.6f; // 1 = sin derrape, 0 = hielo
    [Range(0f,1f)] [SerializeField] float forwardGrip = 1f;
    [Range(0f,1f)] [SerializeField] float handbrakeLateralGrip = 0.6f; // grip lateral cuando tirás freno de mano
    [SerializeField] float extraDragWhenNoThrottle = 0.8f; // frena un toque si soltás el acelerador

    [Header("Frenos")]
    [SerializeField] float brakeStrength = 250f;
    [SerializeField] KeyCode brakeKey = KeyCode.Space;
    [SerializeField] KeyCode handbrakeKey = KeyCode.LeftShift;

    [Header("Rigidbody")]
    [SerializeField] float baseDrag = 0.15f;
    [SerializeField] Vector3 centerOfMassOffset = new Vector3(0,-0.35f,0);
    
    // —— Reacción externa (colisiones/paredes) ——
    [Header("Colisión externa")]
    [SerializeField] float defaultLockSeconds = 0.25f;
    [SerializeField] float defaultExtraBrake = 120f;

    Rigidbody rb;
    float steerInput, throttleInput;
    bool braking, handbrake;
    
    // timers/estados externos
    float externalLockTimer = 0f;
    float externalExtraBrake = 0f;

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
        if (externalLockTimer > 0f)
        {
            // bloqueo suave del input
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
    }

    void FixedUpdate()
    {
        Vector3 vel = rb.linearVelocity;
        Vector3 fwd = transform.forward;

        // Velocidad en XZ
        Vector3 velXZ = new Vector3(vel.x, 0, vel.z);
        float speed = velXZ.magnitude;

        // 1) Aceleración
        float accel = throttleInput >= 0 ? accelForward : accelReverse;
        rb.AddForce(fwd * throttleInput * accel, ForceMode.Acceleration);

        // 2) Clamp de velocidad
        float maxSpeed = throttleInput >= 0 ? maxSpeedForward : maxSpeedReverse;
        Vector3 dirSpeed = Vector3.Project(rb.linearVelocity, fwd);
        Vector3 sideSpeed = rb.linearVelocity - dirSpeed;

        if (dirSpeed.magnitude > maxSpeed)
            dirSpeed = dirSpeed.normalized * maxSpeed;

        // 3) Grip
        float currentLateralGrip = handbrake ? handbrakeLateralGrip : lateralGrip;
        sideSpeed *= currentLateralGrip;
        dirSpeed  *= forwardGrip;
        rb.linearVelocity = dirSpeed + sideSpeed;

        // 4) Dirección dependiente de velocidad
        float speed01 = Mathf.InverseLerp(0f, maxSpeedForward, speed);
        float targetSteer = Mathf.Lerp(steerAngleAt0, steerAngleAtMax, speed01) * steerInput;
        Quaternion steerRot = Quaternion.AngleAxis(targetSteer * Time.fixedDeltaTime * steerResponse, Vector3.up);
        rb.MoveRotation(rb.rotation * steerRot);

        // 5) Auto-align
        if (speed > 0.5f)
        {
            Vector3 heading = Vector3.Slerp(fwd, velXZ.normalized, autoAlign * Time.fixedDeltaTime);
            Quaternion alignRot = Quaternion.LookRotation(heading, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, alignRot, 0.35f * Time.fixedDeltaTime));
        }

        // 6) Frenos
        if (braking)
            rb.AddForce(-rb.linearVelocity.normalized * brakeStrength, ForceMode.Acceleration);
        else if (Mathf.Approximately(throttleInput, 0f))
            rb.linearVelocity *= (1f - (extraDragWhenNoThrottle * Time.fixedDeltaTime));

        // 7) Frenado externo mientras dure el lock
        if (externalLockTimer > 0f)
        {
            rb.AddForce(-rb.linearVelocity.normalized * (brakeStrength + externalExtraBrake), ForceMode.Acceleration);
            externalLockTimer -= Time.fixedDeltaTime;
            if (externalLockTimer <= 0f) externalExtraBrake = 0f;
        }
    }
    
    // —— Compatibilidad con tu Detect_Pared antiguo ——
    public void Frenar() => Frenar(defaultLockSeconds, defaultExtraBrake);

    // Overload para customizar
    public void Frenar(float lockSeconds, float extraBrake)
    {
        externalLockTimer = Mathf.Max(externalLockTimer, lockSeconds);
        externalExtraBrake = Mathf.Max(externalExtraBrake, extraBrake);
    }
}
