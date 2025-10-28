using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Lanzador_v2 : MonoBehaviour
{
    [Header("Tag del enemigo a buscar")]
    [SerializeField] string _Buscar = "Enemy";

    [Header("Misil a instanciar")]
    [SerializeField] GameObject _cohete;
    [Header("Misil Señielo")]
    [SerializeField] GameObject _misil_señuelo;
    [SerializeField] float _offset_Y = 2f;

    [Header("Punto de instanciacion")]
    [SerializeField] Transform _spawnPoint;
    //Rigidbody _rb;

    [Header("Retardo efecto de lanzamiento")]
    [SerializeField] float _apagar_misil = 0.25f;
    private List<Rigidbody> _EnemigosDetectados = new List<Rigidbody>();

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag(_Buscar))
        {
            Debug.Log("enemigo encontrado");
            /*
            Rigidbody rb = other.attachedRigidbody;
            
            if (rb != null && !_EnemigosDetectados.Contains(rb))
            {
                _EnemigosDetectados.Add(rb);
                //Debug.Log($"Entró: {rb.name}");
            }
            */
            GameObject i_cohete = Instantiate(
                _cohete,
                _spawnPoint.position + new Vector3(0f, _offset_Y, 0f),
                _spawnPoint.rotation,
                transform
            );
            i_cohete.SetActive(false);

            //setear las condiciones iniciales para el cohete
            Enemy_v2 enemy = other.GetComponent<Enemy_v2>();
            Missile_v2 misil = i_cohete.GetComponent<Missile_v2>();
            Vector3 initialDir = _spawnPoint.up; // sale en la dirección del spawnPoint
            misil.set_target(enemy, initialDir, _apagar_misil);
            i_cohete.SetActive(true);
            if (_misil_señuelo.activeSelf)
            {
                _misil_señuelo.SetActive(false);
                StartCoroutine(Activar_misil());    
            }
            
        }
    }
/*
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_Buscar))
        {
            Rigidbody rb = other.attachedRigidbody;

            if (rb != null && _EnemigosDetectados.Contains(rb))
            {
                _EnemigosDetectados.Remove(rb);
                //Debug.Log($"Salió: {rb.name}");
            }
            
        }
    }
*/
    /// <summary>
    /// Devuelve todos los Rigidbodies actualmente dentro del collider.
    /// </summary>
    public List<Rigidbody> ObtenerTodos()
    {
        return _EnemigosDetectados;
    }

    
    IEnumerator Activar_misil()
    {
        // Espera la cantidad de segundos indicada
        yield return new WaitForSeconds(_apagar_misil);

        // Cambia la variable a falso
        _misil_señuelo.SetActive(true);

        //Debug.Log("⏰ Tiempo terminado. Variable 'activo' = " + activo);
    }
    
}