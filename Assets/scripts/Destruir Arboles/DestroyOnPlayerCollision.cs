using UnityEngine;

public class DestroyOnPlayerCollision : MonoBehaviour
{
    // Este método se llama automáticamente cuando ocurre una colisión física
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); // Destruye el objeto que tiene este script
        }
    }

    // Si el objeto usa colliders con "isTrigger" activado, usamos este otro método
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
