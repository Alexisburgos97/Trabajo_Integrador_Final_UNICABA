using UnityEngine;
using Simplon;
using System.Collections;

public class LevelRescueSetup : MonoBehaviour
{
    IEnumerator Start()
    {
        // Espera 1 frame por si hay instancias/spawners al cargar la escena
        yield return null;

        var collectibles = FindObjectsByType<Collectible>(
            FindObjectsInactive.Include,     // o Exclude si no querés contar inactivos
            FindObjectsSortMode.None         // None = más rápido
        );

        int total = 0;
        for (int i = 0; i < collectibles.Length; i++)
        {
            if (collectibles[i].type == Collectible.Type.Person)
                total++;
        }

        var gc = GameControler.Instance ?? FindAnyObjectByType<GameControler>();
        if (gc != null)
        {
            gc.Setear_total_a_rescatar(total);
            Debug.Log($"[Nivel] Total a rescatar: {total}");
        }
        else
        {
            Debug.LogError("[LevelRescueSetup] No se encontró GameControler.");
        }
    }
}