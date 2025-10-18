using UnityEngine;

public class PowerUpEscudo : MonoBehaviour
{
    [Header("Duraci�n extra del escudo")]
    public float duracion = 5f;

    //[Header("Efectos")]
    //public GameObject recogerVFX;
    //public AudioClip recogerSFX;
    //public float volumen = 0.8f;
    public void Start()
    {
        // Asegurarse de que el collider es trigger
        var col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("[POWERUP] No hay Collider en el PowerUpEscudo!");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning("[POWERUP] El Collider no es trigger, ajustando...");
            col.isTrigger = true;
        }

        // Asegurarse de que hay un Rigidbody (puede ser cinem�tico)
        var rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("[POWERUP] No hay Rigidbody, a�adiendo uno cinem�tico...");
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var shield = other.GetComponentInChildren<EscudoJugador>();
        if (shield != null)
        {
            shield.duracionEscudo = duracion; // opcional: actualizar duraci�n
            shield.ActivarEscudo();
            Debug.Log("activar escudo");
            //if (recogerVFX)
            //    Instantiate(recogerVFX, transform.position, Quaternion.identity);
            //if (recogerSFX)
            //    AudioSource.PlayClipAtPoint(recogerSFX, transform.position, volumen);

            Destroy(gameObject);
        }
    }
}