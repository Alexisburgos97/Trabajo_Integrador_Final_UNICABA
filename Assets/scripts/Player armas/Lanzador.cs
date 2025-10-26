using System.Collections.Generic;
using UnityEngine;

public class Lanzador : MonoBehaviour
{
    [Header("Detección")]
    public float detectionRadius = 20f;
    public LayerMask enemyLayer; // asignar una layer donde estén los enemigos o usar tag
    public float checkInterval = 0.25f;

    [Header("Cohetes")]
    public GameObject rocketPrefab;
    public int poolSize = 6;
    public Transform spawnPoint; // punto donde salen los cohetes (puede ser el transform del lanzador)
    public float sequentialDelay = 0.15f; // tiempo entre disparos si activas varios

    [Header("Lanzamiento")]
    public float launchSpreadOffset = 0f; // opcional, si quieres offset inicial
    public int maxRocketsPerWave = 3;

    List<GameObject> rocketPool = new List<GameObject>();
    HashSet<Transform> reservedTargets = new HashSet<Transform>();
    float nextCheck;
    Queue<Transform> detectedQueue = new Queue<Transform>();

    void Start()
    {
        // crear pool
        for (int i = 0; i < poolSize; i++)
        {
            var r = Instantiate(rocketPrefab, spawnPoint.position, spawnPoint.rotation, transform);
            r.SetActive(false);
            rocketPool.Add(r);
        }
        nextCheck = Time.time + Random.Range(0f, checkInterval); // para no chequear todos al mismo frame
    }

    void Update()
    {
        if (Time.time >= nextCheck)
        {
            nextCheck = Time.time + checkInterval;
            CheckForEnemiesAndFire();
        }
    }

    void CheckForEnemiesAndFire()
    {
        // limpiar queue anterior
        detectedQueue.Clear();

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var c in hits)
        {
            if (!c.CompareTag("Enemy")) continue;
            Transform t = c.transform;
            if (reservedTargets.Contains(t)) continue; // ya asignado a otro cohete
            detectedQueue.Enqueue(t);
        }

        // lanzar hasta maxRocketsPerWave o hasta que no haya cohetes libres
        int fired = 0;
        while (detectedQueue.Count > 0 && fired < maxRocketsPerWave)
        {
            Transform target = detectedQueue.Dequeue();
            GameObject rocketGo = GetAvailableRocket();
            if (rocketGo == null) break;

            // marcar target reservado
            reservedTargets.Add(target);

            // activar y lanzar
            rocketGo.SetActive(true);
            rocketGo.transform.position = spawnPoint.position;
            rocketGo.transform.rotation = spawnPoint.rotation;

            // inicializar el rocket (asumimos Rocket.cs en el prefab)
            Cohete r = rocketGo.GetComponent<Cohete>();
            Vector3 initialDir = spawnPoint.forward; // sale en la dirección del spawnPoint
            r.Launch(target, initialDir);

            // opcional: cuando el cohete muera, liberar target. Para eso, escuchamos la destrucción via script: usamos un helper
            StartCoroutine(ReleaseTargetWhenRocketDies(r, target));

            fired++;
        }
    }

    System.Collections.IEnumerator ReleaseTargetWhenRocketDies(Cohete rocket, Transform target)
    {
        // esperar hasta que el rocket sea destruido o deje de existir
        while (rocket != null && rocket.gameObject.activeInHierarchy)
            yield return null;

        // liberar target si sigue en el set
        if (reservedTargets.Contains(target))
            reservedTargets.Remove(target);
    }

    GameObject GetAvailableRocket()
    {
        foreach (var r in rocketPool)
        {
            if (!r.activeInHierarchy)
                return r;
        }
        // si no hay libres, podría instanciar más (opcional)
        return null;
    }

    // para visualizar el radio en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
