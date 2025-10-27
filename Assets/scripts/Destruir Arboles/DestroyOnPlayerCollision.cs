using UnityEngine;

public class DestroyOnPlayerCollision : MonoBehaviour
{
    // Este m�todo se llama autom�ticamente cuando ocurre una colisi�n f�sica
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); // Destruye el objeto que tiene este script
        }
    }

    // Si el objeto usa colliders con "isTrigger" activado, usamos este otro m�todo
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
