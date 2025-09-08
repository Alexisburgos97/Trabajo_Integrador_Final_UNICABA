using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
[SerializeField] float _moveSpeed = 10f;      // Velocidad hacia adelante/atrás
    [SerializeField] float _turnSpeed = 100f;     // Velocidad de giro
    private Rigidbody _rb;
    
    private float _moveInput;
    private float _turnInput;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        // Leer input en Update (mejor para no perder frames de input)
        _moveInput = Input.GetAxis("Vertical");
        _turnInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        // Movimiento adelante / atrás
        Vector3 move = transform.forward * _moveInput * _moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + move);
        //avance con fuerza
        // Rotación izquierda / derecha
        float turn = _turnInput * _turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        _rb.MoveRotation(_rb.rotation * turnRotation);
    }
  
}