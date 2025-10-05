using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 10f;      // Velocidad adelante/atrás
    [SerializeField] float _maxSpeed = 8f;     // Velocidad máxima
    [SerializeField] float _turnSpeed = 100f;     // Velocidad de giro
    private Rigidbody _rb;

    private float _moveInput;
    private float _turnInput;

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
    if (_frenado) return;

    // Fuerza adelante/atrás
    _rb.AddForce(transform.forward * _moveInput * _moveSpeed, ForceMode.Acceleration);

    // --- Frenar deslizamiento lateral ---
    Vector3 localVel = transform.InverseTransformDirection(_rb.linearVelocity);
    localVel.x *= 0.9f; // amortiguamos el movimiento lateral (1 = hielo, 0 = pega al suelo)
    _rb.linearVelocity = transform.TransformDirection(localVel);

    // Limitar velocidad máxima en XZ
    Vector3 vel = _rb.linearVelocity;
    Vector3 velXZ = new Vector3(vel.x, 0f, vel.z);
    if (velXZ.magnitude > _maxSpeed)
    {
        velXZ = velXZ.normalized * _maxSpeed;
        _rb.linearVelocity = new Vector3(velXZ.x, vel.y, velXZ.z);
    }

    // Rotación (solo si hay movimiento)
    if (_moveInput != 0)
    {
        float turn = _turnInput * _turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        _rb.MoveRotation(_rb.rotation * turnRotation);
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