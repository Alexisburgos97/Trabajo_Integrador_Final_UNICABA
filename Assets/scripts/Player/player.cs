using UnityEngine;

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
    */    
        
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
  
}