using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    public float moveSpeed = 10f;      // Velocidad hacia adelante/atrás
    public float turnSpeed = 100f;     // Velocidad de giro
    public float jumpForce = 5f;       // Fuerza del salto
    public float groundCheckDistance = 1.1f; // Distancia para verificar si está en el suelo

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; 
    }

    void Update()
    {
        // Movimiento adelante / atrás
        float moveInput = Input.GetAxis("Vertical");   // W/S o Flechas ↑↓
        Vector3 move = transform.forward * moveInput * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + move);

        // Rotación izquierda / derecha
        float turnInput = Input.GetAxis("Horizontal"); // A/D o Flechas ←→
        float turn = turnInput * turnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        // Verificar si está en el suelo
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        // Salto con SPACE
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}