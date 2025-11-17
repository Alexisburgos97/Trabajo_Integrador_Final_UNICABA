using UnityEngine;
using System.Collections;

public class PowerUpEscudo : MonoBehaviour
{
    [Header("Duración del escudo")]
    [SerializeField] float duracion = 5f;
    [SerializeField] float _Reactivacion = 30f;
    [SerializeField] GameObject _marcador,_marca_map;

    Collider _collider;
    ParticleSystem _particles;
    Rigidbody _rb;

    void Start()
    {
        _collider = GetComponent<Collider>();
        _particles = GetComponent<ParticleSystem>();
        _rb = GetComponent<Rigidbody>();

        if (_collider == null)
            Debug.LogError("[POWERUP] No hay Collider!");
        else if (!_collider.isTrigger)
            _collider.isTrigger = true;

        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody>();
            _rb.isKinematic = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var shield = other.GetComponentInChildren<EscudoJugador>();
        if (shield != null)
        {
            // aplicar escudo
            shield.duracionEscudo = duracion;
            shield.ActivarEscudo();
            Debug.Log("Escudo ACTIVADO");

            // ocultar power-up temporalmente
            Desactivar();

            // reactivarlo luego
            StartCoroutine(Reactivar());
        }
    }

    void Desactivar()
    {
        _collider.enabled = false;

        if (_particles != null)
            _particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        /*
        if (_rb != null)
        {
            _rb.isKinematic = true;
            //_rb.velocity = Vector3.zero;
        }*/
        if(_marcador!=null)_marcador.SetActive(false);
        if(_marca_map!=null)_marca_map.SetActive(false);
    }

    IEnumerator Reactivar()
    {
        yield return new WaitForSecondsRealtime(_Reactivacion);

        _collider.enabled = true;

        if (_particles != null)
            _particles.Play();
        
        if(_marcador!=null)_marcador.SetActive(true);
        if(_marca_map!=null)_marca_map.SetActive(true);

        Debug.Log("PowerUp REACTIVADO");
    }
}
