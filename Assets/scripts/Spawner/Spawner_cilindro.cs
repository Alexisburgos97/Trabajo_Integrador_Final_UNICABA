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
            // Posición y rotación base
            Vector3 spawnPos = _spawnPoint != null ? _spawnPoint.position : transform.position;
            Quaternion spawnRot = _spawnPoint != null ? _spawnPoint.rotation : transform.rotation;

            // Instanciamos
            GameObject spawned = Instantiate(_cilindroPrefab, spawnPos, spawnRot);

            // --- Alineamos el FORWARD del cilindro con la LEFT del spawnPoint ---
            // Calculamos la dirección "left" en world space (si no hay spawnPoint, usamos el mismo spawner)
            Vector3 leftDirWorld;
            Vector3 upVector;

            if (_spawnPoint != null)
            {
                leftDirWorld = _spawnPoint.TransformDirection(Vector3.left); // izquierda local -> world
                upVector = _spawnPoint.up;
            }
            else
            {
                leftDirWorld = transform.TransformDirection(Vector3.left);
                upVector = transform.up;
            }

            // Si por alguna razón leftDirWorld es casi cero, evitamos LookRotation (por seguridad)
            if (leftDirWorld.sqrMagnitude > 0.0001f)
            {
                // Forzamos la rotación del objeto instanciado para que su forward apunte en leftDirWorld
                spawned.transform.rotation = Quaternion.LookRotation(leftDirWorld, upVector);
                // Alternativa más directa: spawned.transform.forward = leftDirWorld.normalized;
            }

            // Resto de control de uso único / cooldown
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
