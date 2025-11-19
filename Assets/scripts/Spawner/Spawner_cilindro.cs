using UnityEngine;
using System.Collections;

public class Spawner_cilindro : MonoBehaviour
{
    [Header("Configuración del prefab y spawn")]
    [SerializeField] private GameObject _cilindroPrefab; // Prefab del cilindro trampa
    [SerializeField] private Transform _spawnPoint;      // Punto donde aparecerá el cilindro
    [SerializeField] private bool _usarUnaVez = true;    // Solo se activa una vez

    [Header("Configuración de enemigos")]
    [SerializeField, Min(1)] private int _cantidadEnemigos = 1; // Mínimo 1 enemigo

    [Header("Tiempo de respawn")]
    [SerializeField] private float _cool_down = 30f;             // Tiempo base entre activaciones
    [SerializeField] private float _tiempoMin = 1f;              // Mínimo tiempo entre spawns
    [SerializeField] private float _tiempoMax = 3f;              // Máximo tiempo entre spawns

    // --- Mostrar Gizmo del trigger en modo edición ---
    [Header("Activar gizmo")]
    [SerializeField] private bool _mostrarGizmo = true;
    private float _espera = 0;
    private bool _activado = false;
    private bool _spawneando = false;

    void Update()
    {
        espera_respown();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_activado)
        {
            _activado = true;

            if (_usarUnaVez)
            {
                // Instanciar directamente una vez
                StartCoroutine(SpawnMultiple());
            }
            else
            {
                // Inicia la secuencia y luego se reactivará automáticamente
                StartCoroutine(SpawnMultiple());
                _espera = _cool_down;
            }
        }
    }

    private IEnumerator SpawnMultiple()
    {
        if (_spawneando) yield break; // Evita solaparse
        _spawneando = true;

        for (int i = 0; i < _cantidadEnemigos; i++)
        {
            SpawnCilindro();

            // Espera un tiempo aleatorio entre cada spawn
            if (i < _cantidadEnemigos - 1)
                yield return new WaitForSeconds(Random.Range(_tiempoMin, _tiempoMax));
        }

        _spawneando = false;
    }

    private void SpawnCilindro()
    {
        // Posición y rotación base
        Vector3 spawnPos = _spawnPoint != null ? _spawnPoint.position : transform.position;
        Quaternion spawnRot = _spawnPoint != null ? _spawnPoint.rotation : transform.rotation;

        // Instanciamos el prefab
        GameObject spawned = Instantiate(_cilindroPrefab, spawnPos, spawnRot);

        // --- Alineamos el FORWARD del cilindro con la LEFT del spawnPoint ---
        Vector3 leftDirWorld;
        Vector3 upVector;

        if (_spawnPoint != null)
        {
            leftDirWorld = _spawnPoint.TransformDirection(Vector3.left);
            upVector = _spawnPoint.up;
        }
        else
        {
            leftDirWorld = transform.TransformDirection(Vector3.left);
            upVector = transform.up;
        }

        if (leftDirWorld.sqrMagnitude > 0.0001f)
        {
            spawned.transform.rotation = Quaternion.LookRotation(leftDirWorld, upVector);
        }
    }

    // --- Reactivación del spawner ---
    private void espera_respown()
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
    private void OnDrawGizmos()
    {
        if (!_mostrarGizmo) return;
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f); // Verde transparente

        Collider col = GetComponent<Collider>();
        if (col == null) return;

        Gizmos.matrix = col.transform.localToWorldMatrix;

        if (col is BoxCollider box)
        {
            Gizmos.DrawCube(box.center, box.size);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(box.center, box.size);
        }
        else if (col is SphereCollider sphere)
        {
            Gizmos.DrawSphere(sphere.center, sphere.radius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(sphere.center, sphere.radius);
        }
        else if (col is CapsuleCollider capsule)
        {
            // Dibujar cápsula aproximada
            Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
            float height = capsule.height;
            float radius = capsule.radius;

            Vector3 center = capsule.center;
            int direction = capsule.direction; // 0 = X, 1 = Y, 2 = Z

            // Dibujo aproximado con dos esferas + cilindro
            Vector3 dir = direction == 0 ? Vector3.right :
                        direction == 1 ? Vector3.up : Vector3.forward;

            float cylinderHeight = height - (radius * 2);

            // Parte central
            Gizmos.DrawCube(center, Vector3.one * radius * 2 + dir * cylinderHeight);

            // Tapas
            Gizmos.DrawSphere(center + dir * (cylinderHeight / 2), radius);
            Gizmos.DrawSphere(center - dir * (cylinderHeight / 2), radius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(center + dir * (cylinderHeight / 2), radius);
            Gizmos.DrawWireSphere(center - dir * (cylinderHeight / 2), radius);
        }
    }
}
