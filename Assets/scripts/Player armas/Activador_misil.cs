using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Activador_misil : MonoBehaviour
{

  [Header("Referencia al objeto que se activará")]
    [SerializeField] GameObject objetoADespertar;

    [Header("Tag que debe activar el objeto")]
    [SerializeField] string tagActivador = "Player";

    [Header("Sonido al activar el power up")]
    [SerializeField] AudioSource sonido_activador;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagActivador))
        {
            if (sonido_activador != null) sonido_activador.Play();
            
            if (objetoADespertar != null && !objetoADespertar.activeSelf)
            {
                objetoADespertar.SetActive(true);
                Debug.Log($"{objetoADespertar.name} fue activado por {other.name}");
            }
            StartCoroutine(DestruirConDelay());
        }
    }

    IEnumerator DestruirConDelay()
    {
        yield return new WaitForFixedUpdate(); // espera al final del frame de física
        Destroy(gameObject);
    }
}
