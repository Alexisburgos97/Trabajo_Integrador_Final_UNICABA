using UnityEngine;

public class Detect_Pared : MonoBehaviour
{
    [SerializeField] string _tag;      // Tag de la "pared"
    [SerializeField] float retroceso = 0.5f; // Distancia de retroceso
    private TopDownCarController _controller;
    private Rigidbody _rb;

    private void Start()
    {
        _controller = GetComponent<TopDownCarController>();
        _rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_tag))
        {
            // Llamamos al freno
            _controller.Frenar();

            // Retroceso en dirección opuesta a donde mira el coche
            Vector3 direccionRetroceso = -transform.forward * retroceso;

            // Movemos un poco el coche hacia atrás (teleport suave con MovePosition)
            _rb.MovePosition(_rb.position + direccionRetroceso);
            _controller.Reanudar();
        }
    }
}
