using UnityEngine;

public class Spawner_cilindro : MonoBehaviour
{
    [SerializeField] private GameObject _cilindroPrefab; // Prefab del cilindro trampa
    [SerializeField] private Transform _spawnPoint;      // Punto donde aparecerá el cilindro
    [SerializeField] private bool _usarUnaVez = true;    // Solo se activa una vez

    private bool _activado = false;

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
        }
    }
}
