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
    [SerializeField] float _Reactivacion=30f;
    [SerializeField] GameObject _marcador,_marca_map,_obj_x,_obj_y;
    Collider _collider;

    void Start()
    {
        _collider = GetComponent<Collider>();
    }
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
            StartCoroutine(ReactivarConDelay());
            Desactivar();
        }
    }

    void Desactivar()
    {
        _collider.enabled = false;
        if(_marcador!=null)_obj_x.SetActive(false);
        if(_marcador!=null)_obj_y.SetActive(false);
        if(_marcador!=null)_marcador.SetActive(false);
        if(_marca_map!=null)_marca_map.SetActive(false);
    }

    IEnumerator ReactivarConDelay()
    {
        yield return new WaitForSecondsRealtime(_Reactivacion);; // espera al final del frame de física
        _collider.enabled = true;
        if(_marcador!=null)_obj_x.SetActive(true);
        if(_marcador!=null)_obj_y.SetActive(true);
        if(_marcador!=null)_marcador.SetActive(true);
        if(_marca_map!=null)_marca_map.SetActive(true);
    }
}
