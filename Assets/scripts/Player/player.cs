using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 10f;      // Velocidad hacia adelante/atrás
    [SerializeField] float _turnSpeed = 100f;     // Velocidad de giro
    [SerializeField] float _jumpForce = 5f;       // Fuerza del salto
    [SerializeField] float _groundCheckDistance = 1.1f; // Distancia para verificar si está en el suelo
    [SerializeField] LayerMask _capa_suelo; //muestra las capas diponibles para seleccionar la indicada por inspector
    private Rigidbody _rb;
    private bool _isGrounded;
    float _moveInput,_turnInput,_turn;
    Vector3 _move;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        // Movimiento adelante / atrás
        _moveInput = Input.GetAxis("Vertical");   // W/S o Flechas ↑↓
        _move = transform.forward * _moveInput * _moveSpeed * Time.deltaTime;
        _rb.MovePosition(_rb.position + _move);

        // Rotación izquierda / derecha
      
        _turnInput = Input.GetAxis("Horizontal"); // A/D o Flechas ←→
        _turn = _turnInput * _turnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, _turn, 0f);

        _rb.MoveRotation(_rb.rotation * turnRotation);
        // Verificar si está en el suelo
        _isGrounded = Physics.Raycast(transform.position, Vector3.up, _groundCheckDistance, _capa_suelo);

    }

    
    void FixUpadate()
    {
        // Salto con SPACE
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Salto");
            Debug.Log(_isGrounded);
            if (_isGrounded)
            {
                Debug.Log("en tierra");
            }
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

        }
    }
    
}