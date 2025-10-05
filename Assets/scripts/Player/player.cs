using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 10f;      // Velocidad adelante/atrás
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