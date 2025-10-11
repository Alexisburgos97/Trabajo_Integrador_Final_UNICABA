using System.Collections;
using UnityEngine;

/// <summary>
/// BlinkingObject: controla un parpadeo ON/OFF robusto sobre TODOS los Renderers hijos.
/// Exponer métodos públicos para que un manager ajuste intervalo y arranque / pare el parpadeo.
/// Si no hay Renderers hace fallback con SetActive.
/// </summary>
public class BlinkingObject : MonoBehaviour
{
    [Tooltip("Intervalo en segundos entre toggles (tiempo entre cambios de visible/invisible).")]
    public float blinkInterval = 0.5f;

    private Coroutine blinkCoroutine;
    private Renderer[] renderers;
    private bool hasRenderers;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        hasRenderers = renderers != null && renderers.Length > 0;
    }

    IEnumerator BlinkRoutine()
    {
        // Empezamos con visible = true para que el primer toggle lo apague
        bool visible = true;
        while (true)
        {
            visible = !visible;
            SetVisibleInternal(visible);
            yield return new WaitForSeconds(Mathf.Max(0.01f, blinkInterval));
        }
    }

    // Inicia o reanuda el parpadeo (no lo vuelve a instanciar si ya está corriendo)
    public void StartBlinking()
    {
        if (blinkCoroutine == null)
            blinkCoroutine = StartCoroutine(BlinkRoutine());
    }

    // Para el parpadeo y deja el objeto en el estado "makeVisible"
    public void StopBlinking(bool makeVisible = true)
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
        SetVisibleInternal(makeVisible);
    }

    // Cambia el intervalo (en segundos). Si parpadea, el nuevo intervalo se aplicará a la siguiente espera.
    public void SetBlinkInterval(float interval)
    {
        blinkInterval = Mathf.Max(0.01f, interval);
    }

    // Fuerza visible/invisible (detiene el parpadeo)
    public void SetVisible(bool visible)
    {
        StopBlinking();
        SetVisibleInternal(visible);
    }

    private void SetVisibleInternal(bool visible)
    {
        if (hasRenderers)
        {
            foreach (var r in renderers)
                if (r != null)
                    r.enabled = visible;
        }
        else
        {
            gameObject.SetActive(visible);
        }
    }

    void OnDestroy()
    {
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);
    }
}
