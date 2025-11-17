using System.Collections;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum Type { Coin, Gasoline, Person }   // agregamos Person
    public Type type;

    [SerializeField] GameObject _marcador,_marca_map;
    [SerializeField] float _Reactivacion=30;
    [SerializeField] MeshRenderer _meshRenderer;
    Collider _collider;
    void Start()
    {
        _collider = GetComponent<Collider>();
        //_meshRenderer=GetComponent<MeshRenderer>();

        if (_collider == null)
            Debug.LogError("[POWERUP] No hay Collider!");
        else if (!_collider.isTrigger)
            _collider.isTrigger = true;
        

    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("interaccion colecctable");
        var car = other.GetComponent<Colectables>();
        Debug.Log(car);
        if (car == null) return;

        car.Collect(type);
        if (type == Type.Gasoline)
        {
            StartCoroutine(ReactivarConDelay());
            Desactivar();
        }
        else
        {
            Destroy(gameObject);    
        }
        
    }

    void Desactivar()
    {
        _collider.enabled = false;
        _meshRenderer.enabled=false;
        if(_marcador!=null)_marcador.SetActive(false);
        if(_marca_map!=null)_marca_map.SetActive(false);
    }

    IEnumerator ReactivarConDelay()
    {
        yield return new WaitForSecondsRealtime(_Reactivacion);; // espera al final del frame de física
        _collider.enabled = true;
        _meshRenderer.enabled=true;
        if(_marcador!=null)_marcador.SetActive(true);
        if(_marca_map!=null)_marca_map.SetActive(true);
    }
}