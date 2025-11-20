using UnityEngine;
using System.Collections;

public class Spawner_cilindro : MonoBehaviour
{
    [Header("Configuración del prefab y spawn")]
    [SerializeField] private GameObject _cilindroPrefab; 
    [SerializeField] private Transform _spawnPoint;      
    [SerializeField] private bool _usarUnaVez = true;    

    [Header("Configuración de enemigos")]
    [SerializeField, Min(1)] private int _cantidadEnemigos = 1;

    [Header("Tiempo de respawn")]
    [SerializeField] private float _cool_down = 30f;
    [SerializeField] private float _tiempoMin = 1f;
    [SerializeField] private float _tiempoMax = 3f;

    [Header("Enlazar con otros spawners")]
    [SerializeField] private Spawner_cilindro[] _spawnersEnlazados; 

    [Header("Activar gizmo")]
    [SerializeField] private bool _mostrarGizmo = true;


    // ------------------------------------------------------------
    // 🔥 NUEVA SECCIÓN: DETECCIÓN DIRECCIONAL
    // ------------------------------------------------------------
    public enum EjeDeteccion { X, Y, Z }
    public enum DireccionDeteccion { Positiva, Negativa }

    [Header("Detección Direccional del Jugador")]
    [SerializeField] private EjeDeteccion _ejeDeteccion = EjeDeteccion.Z;
    [SerializeField] private DireccionDeteccion _direccion = DireccionDeteccion.Positiva;
    [SerializeField, Range(0f, 180f)] private float _anguloTolerancia = 45f;

    private Vector3 _prevPlayerPos;
    private bool _tienePrevPos = false;

    // ------------------------------------------------------------

    private float _espera = 0;
    private bool _activado = false;
    private bool _spawneando = false;

    void Update()
    {
        espera_respown();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || _activado)
            return;

        // Guardar posición previa si es primera vez
        if (!_tienePrevPos)
        {
            _prevPlayerPos = other.transform.position;
            _tienePrevPos = true;
            return;
        }

        // Calcular dirección de movimiento del jugador
        Vector3 movimiento = other.transform.position - _prevPlayerPos;

        if (DetectaDireccionCorrecta(movimiento))
        {
            ActivarSpawner();
        }

        _prevPlayerPos = other.transform.position;
    }

    // ------------------------------------------------------------
    // 🔥 Lógica de detección en la dirección indicada
    // ------------------------------------------------------------
    private bool DetectaDireccionCorrecta(Vector3 mov)
    {
        if (mov.sqrMagnitude < 0.0001f)
            return false; // no se movió

        mov.Normalize();

        Vector3 eje = Vector3.forward; // por defecto Z+

        switch (_ejeDeteccion)
        {
            case EjeDeteccion.X: eje = Vector3.right; break;
            case EjeDeteccion.Y: eje = Vector3.up; break;
            case EjeDeteccion.Z: eje = Vector3.forward; break;
        }

        if (_direccion == DireccionDeteccion.Negativa)
            eje = -eje;

        float angulo = Vector3.Angle(mov, eje);

        return angulo <= _anguloTolerancia;
    }

    // ------------------------------------------------------------

    private void ActivarSpawner()
    {
        if (_activado) return;
        _activado = true;

        StartCoroutine(SpawnMultiple());

        foreach (var spawner in _spawnersEnlazados)
            if (spawner != null)
                spawner.ActivarSpawner();

        if (!_usarUnaVez)
            _espera = _cool_down;
    }

    private IEnumerator SpawnMultiple()
    {
        if (_spawneando) yield break; 
        _spawneando = true;

        for (int i = 0; i < _cantidadEnemigos; i++)
        {
            SpawnCilindro();

            if (i < _cantidadEnemigos - 1)
                yield return new WaitForSeconds(Random.Range(_tiempoMin, _tiempoMax));
        }

        _spawneando = false;
    }

    private void SpawnCilindro()
    {
        Vector3 spawnPos = _spawnPoint != null ? _spawnPoint.position : transform.position;
        Quaternion spawnRot = _spawnPoint != null ? _spawnPoint.rotation : transform.rotation;

        GameObject spawned = Instantiate(_cilindroPrefab, spawnPos, spawnRot);

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

    private void espera_respown()
    {
        if (!_usarUnaVez)
        {
            if (_espera > 0)
                _espera -= Time.deltaTime;
            else
                _activado = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (!_mostrarGizmo) return;

        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);

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
            Gizmos.color = new Color(0f, 1f, 0f, 0.2f);

            float height = capsule.height;
            float radius = capsule.radius;

            Vector3 center = capsule.center;
            int direction = capsule.direction;

            Vector3 dir = direction == 0 ? Vector3.right :
                        direction == 1 ? Vector3.up : Vector3.forward;

            float cylinderHeight = height - (radius * 2);

            Gizmos.DrawCube(center, Vector3.one * radius * 2 + dir * cylinderHeight);
            Gizmos.DrawSphere(center + dir * (cylinderHeight / 2), radius);
            Gizmos.DrawSphere(center - dir * (cylinderHeight / 2), radius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(center + dir * (cylinderHeight / 2), radius);
            Gizmos.DrawWireSphere(center - dir * (cylinderHeight / 2), radius);
        }
    }
}
