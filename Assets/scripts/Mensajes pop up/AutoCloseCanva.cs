using System.Collections;
using UnityEngine;

public class AutoCloseCanvasFade : MonoBehaviour
{
    [Header("Tiempo total visible antes de empezar el fade")]
    [SerializeField] private float _tiempoVisible = 3f;

    [Header("Duración del fade out")]
    [SerializeField] private float _duracionFade = 1f;

    private CanvasGroup _canvasGroup;
    private Coroutine _rutina;

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void OnEnable()
    {
        _canvasGroup.alpha = 1f;

        // Reiniciar corrutina si se reactiva el canvas
        if (_rutina != null)
            StopCoroutine(_rutina);

        _rutina = StartCoroutine(CerrarCanvas());
    }

    private IEnumerator CerrarCanvas()
    {
        // 1) Esperar el tiempo visible completo
        yield return new WaitForSeconds(_tiempoVisible);

        // 2) Fade-out suave
        float t = 0f;

        while (t < _duracionFade)
        {
            t += Time.deltaTime;
            float factor = t / _duracionFade;

            _canvasGroup.alpha = 1f - factor;
            yield return null;
        }

        // 3) Asegurar alpha en 0
        _canvasGroup.alpha = 0f;

        // 4) Desactivar el objeto
        gameObject.SetActive(false);
    }
}
