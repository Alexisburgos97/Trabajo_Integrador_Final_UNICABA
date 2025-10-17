using UnityEngine;

public class Spawner_cilindro : MonoBehaviour
{
    [SerializeField] private GameObject _cilindroPrefab; // Prefab del cilindro trampa
    [SerializeField] private Transform _spawnPoint;      // Punto donde aparecerá el cilindro
    [SerializeField] private bool _usarUnaVez = true;    // Solo se activa una vez

    [SerializeField] float _cool_down = 30;
    float _espera=0;
    private bool _activado = false;

    void Update()
    {
        espera_respown();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_activado)
        {
            // Si no asignás un spawnPoint, usa la posición del trigger
            Vector3 spawnPos = _spawnPoint != null ? _spawnPoint.position : transform.position;
            Quaternion spawnRot = _spawnPoint != null ? _spawnPoint.rotation : transform.rotation;

            Instantiate(_cilindroPrefab, spawnPos, spawnRot);

            if (_usarUnaVez)
                _activado = true;
            else
            {
                _activado = true;
                _espera = _cool_down;
            }


        }
    }
    
    //funcion para checkear y reactivar el spawner luego del tiempo de espera
    //si esta configurado para usarse mas de una vez
    void espera_respown()
    {
        if (!_usarUnaVez)
        {
            if (_espera > 0)
            {
                _espera -= Time.deltaTime;
            }
            else
            {
                _activado = false;
            }
        }
    }
}
