using System.Collections;
using UnityEngine;
using TMPro;

public class AutoCloseCanvasFade : MonoBehaviour
{
    [Header("Tiempo visible antes del fade")]
    [SerializeField] private float _tiempoVisible = 3f;

    [Header("Duración del fade out")]
    [SerializeField] private float _duracionFade = 1f;

    [Header("Texto opcional para mostrar cuenta regresiva")]
    [SerializeField] private TextMeshProUGUI _textoTemporizador;

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

        if (_rutina != null)
            StopCoroutine(_rutina);

        _rutina = StartCoroutine(CerrarCanvas());
    }

    private IEnumerator CerrarCanvas()
    {
        float tiempoRestante = _tiempoVisible;

        // 🔹 1) Conteo regresivo mientras el canvas está visible
        while (tiempoRestante > 0f)
        {
            tiempoRestante -= Time.deltaTime;

            if (_textoTemporizador != null)
            {
                // Mostramos en segundos enteros
                int segundos = Mathf.CeilToInt(tiempoRestante);
                _textoTemporizador.text = segundos.ToString();
            }

            yield return null;
        }

        // 🔹 2) Fade-out suave
        float t = 0f;

        while (t < _duracionFade)
        {
            t += Time.deltaTime;

            float factor = t / _duracionFade;
            _canvasGroup.alpha = 1f - factor;

            yield return null;
        }

        _canvasGroup.alpha = 0f;

        // 🔹 3) Desactivar canvas
        gameObject.SetActive(false);
    }
}
