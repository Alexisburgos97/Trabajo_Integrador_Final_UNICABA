using UnityEngine;
using Simplon;

[DefaultExecutionOrder(-10000)]
[DisallowMultipleComponent]
public class Bootstrapper : MonoBehaviour
{
    [Header("Opcional: usar un prefab propio de GameControler")]
    public GameObject gameControlerPrefab;

    [Header("Feedback")]
    public bool logInfo = true;

    void Awake()
    {
        if (GameControlerExists())
        {
            if (logInfo) Debug.Log("[Bootstrapper] Ya existe GameControler en la escena. Nada que hacer.");
            return;
        }

        // Crear uno nuevo
        GameObject go;
        if (gameControlerPrefab != null)
        {
            go = Instantiate(gameControlerPrefab);
            go.name = "GameControler";
        }
        else
        {
            go = new GameObject("GameControler");
            go.AddComponent<GameControler>();
        }

        if (logInfo) Debug.Log("[Bootstrapper] GameControler creado por Bootstrapper.");
    }

    bool GameControlerExists()
    {
        // Cubre ambos casos: el singleton ya seteado o un GC en escena sin inicializar.
        if (GameControler.Instance != null) return true;
        return FindObjectOfType<GameControler>() != null;
    }
}