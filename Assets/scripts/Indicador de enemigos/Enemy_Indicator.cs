using System.Collections.Generic;
using UnityEngine;

public class EnemyIndicatorsManager : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public Camera mainCamera;

    [System.Serializable]
    public class TagPrefabPair
    {
        public string tag;
        public GameObject prefab;
    }

    [Header("Diccionario de indicadores por tag")]
    public List<TagPrefabPair> tagPrefabPairs = new List<TagPrefabPair>();

    [Header("Configuración de visibilidad")]
    public float minDistance = 3f;      // por debajo de esto, ocultar indicador (ej: contacto)
    public float noBlinkDistance = 7f;  // por debajo de esta distancia: indicador visible y SIN parpadeo
    public float maxDistance = 60f;     // por encima de esto, ocultar indicador
    public bool hideWhenOnScreen = true;

    [Header("Configuración del indicador")]
    public float indicatorRadius = 5f;

    [Header("Configuración de actualización")]
    public float updateRate = 0.25f;

    [Header("Parpadeo (intervalos en segundos)")]
    public float blinkIntervalFar = 1.0f;   // intervalo cuando está lejos (parpadeo lento)
    public float blinkIntervalNear = 0.08f; // intervalo cuando está cerca (parpadeo rápido)

    // Diccionarios internos
    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();
    private Dictionary<Transform, GameObject> indicators = new Dictionary<Transform, GameObject>();

    private float updateTimer;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Convertir lista del inspector en diccionario rápido
        foreach (var pair in tagPrefabPairs)
        {
            if (!string.IsNullOrEmpty(pair.tag) && pair.prefab != null && !prefabDictionary.ContainsKey(pair.tag))
                prefabDictionary.Add(pair.tag, pair.prefab);
        }

        RefreshEnemyList();
    }

    void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateRate)
        {
            updateTimer = 0f;
            RefreshEnemyList();
        }

        UpdateIndicators();
    }

    void RefreshEnemyList()
    {
        // Buscar enemigos por cada tag del diccionario
        foreach (var kv in prefabDictionary)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(kv.Key);
            foreach (GameObject enemyObj in enemies)
            {
                Transform enemy = enemyObj.transform;
                if (!indicators.ContainsKey(enemy))
                {
                    GameObject prefab = kv.Value;
                    if (prefab == null)
                    {
                        Debug.LogWarning($"No hay prefab asignado para el tag {kv.Key}");
                        continue;
                    }

                    GameObject arrow = Instantiate(prefab, transform);
                    // Asegurar que exista BlinkingObject (si el prefab no lo trae)
                    if (arrow.GetComponent<BlinkingObject>() == null)
                        arrow.AddComponent<BlinkingObject>();

                    indicators.Add(enemy, arrow);
                }
            }
        }

        // Eliminar referencias a enemigos destruidos
        List<Transform> toRemove = new List<Transform>();
        foreach (var kvp in indicators)
        {
            if (kvp.Key == null)
                toRemove.Add(kvp.Key);
        }
        foreach (var r in toRemove)
        {
            if (indicators[r] != null)
                Destroy(indicators[r]);
            indicators.Remove(r);
        }
    }

    void UpdateIndicators()
    {
        foreach (var kvp in indicators)
        {
            Transform enemy = kvp.Key;
            GameObject arrow = kvp.Value;
            if (enemy == null || arrow == null) continue;

            Vector3 toEnemy = enemy.position - player.position;
            float distance = toEnemy.magnitude;

            // Ocultar por distancia extrema
            if (distance < minDistance || distance > maxDistance)
            {
                SetRenderersVisible(arrow, false);
                // Aseguramos que el blinking esté parado
                var bl = arrow.GetComponent<BlinkingObject>();
                if (bl != null) bl.StopBlinking(false);
                continue;
            }

            // Ocultar si está en pantalla (opcional)
            if (hideWhenOnScreen && IsTargetVisible(enemy.position))
            {
                SetRenderersVisible(arrow, false);
                var bl = arrow.GetComponent<BlinkingObject>();
                if (bl != null) bl.StopBlinking(false);
                continue;
            }

            // Posición y rotación del indicador
            Vector3 dir = toEnemy.normalized;
            Vector3 pos = player.position + dir * indicatorRadius;
            pos.y = player.position.y + 0.5f;
            arrow.transform.position = pos;
            arrow.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

            // Control del parpadeo:
            var blinking = arrow.GetComponent<BlinkingObject>();
            if (blinking == null)
            {
                blinking = arrow.AddComponent<BlinkingObject>();
            }

            if (distance <= noBlinkDistance)
            {
                // Dentro del rango cercano: dejar visible y sin parpadeo
                blinking.StopBlinking(true);
            }
            else
            {
                // Entre noBlinkDistance y maxDistance -> parpadeo variable
                float t = Mathf.InverseLerp(noBlinkDistance, maxDistance, distance); // 0 = near, 1 = far
                // mapear t a interval: near -> blinkIntervalNear (rápido, pequeño), far -> blinkIntervalFar (lento, grande)
                float interval = Mathf.Lerp(blinkIntervalNear, blinkIntervalFar, t);
                blinking.SetBlinkInterval(interval);
                blinking.StartBlinking();
            }
        }
    }

    // Helper: activa/desactiva todos los Renderers hijos; si no hay, usa SetActive
    void SetRenderersVisible(GameObject arrow, bool state)
    {
        var rs = arrow.GetComponentsInChildren<Renderer>(true);
        if (rs != null && rs.Length > 0)
        {
            foreach (var r in rs)
                if (r != null)
                    r.enabled = state;
        }
        else
        {
            arrow.SetActive(state);
        }
    }

    bool IsTargetVisible(Vector3 worldPos)
    {
        Vector3 vp = mainCamera.WorldToViewportPoint(worldPos);
        return vp.z > 0 && vp.x > 0 && vp.x < 1 && vp.y > 0 && vp.y < 1;
    }
}

