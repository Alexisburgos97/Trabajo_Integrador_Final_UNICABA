using UnityEngine;
using System.Collections;

public class EscudoJugador : MonoBehaviour
{
    public bool EscudoActivoGlobal = false; // 🔥 NUEVO: visible para todos

    [Header("Configuración del Escudo")]
    public GameObject visual;
    public AudioSource audioActivacion;
    public float duracionEscudo = 5f;
    public float tiempoCrecer = 0.3f;
    public float tiempoAchicar = 0.2f;
    public float parpadeoAntes = 1.0f;
    public float frecuenciaParpadeo = 0.1f;

    private bool _activo = false;
    private Vector3 _escalaOriginal;

    void Start()
    {
        if (visual != null)
        {
            _escalaOriginal = visual.transform.localScale;
            visual.transform.localScale = Vector3.zero;
            visual.SetActive(false);
        }
    }

    public void ActivarEscudo()
    {
        if (_activo)
        {
            CancelInvoke(nameof(DesactivarEscudo));
            Invoke(nameof(DesactivarEscudo), duracionEscudo);
            return;
        }

        _activo = true;
        EscudoActivoGlobal = true; // 🔥 Activar global

        if (visual != null)
        {
            visual.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(CrecerEscudo());
        }

        if (audioActivacion != null)
            audioActivacion.Play();

        Invoke(nameof(DesactivarEscudo), duracionEscudo);
    }

    private IEnumerator CrecerEscudo()
    {
        float t = 0f;
        while (t < tiempoCrecer)
        {
            t += Time.deltaTime;
            visual.transform.localScale = Vector3.Lerp(Vector3.zero, _escalaOriginal, t / tiempoCrecer);
            yield return null;
        }
        visual.transform.localScale = _escalaOriginal;
    }

    private void DesactivarEscudo()
    {
        _activo = false;
        EscudoActivoGlobal = false; // 🔥 Desactivar global

        if (visual != null)
            StartCoroutine(AchicarYParpadear());
    }

    private IEnumerator AchicarYParpadear()
    {
        float tiempoTotal = parpadeoAntes;
        float contador = 0f;
        bool visible = true;

        while (contador < tiempoTotal)
        {
            visible = !visible;
            visual.SetActive(visible);
            contador += frecuenciaParpadeo;
            yield return new WaitForSeconds(frecuenciaParpadeo);
        }

        visual.SetActive(true);

        float t = 0f;
        Vector3 inicio = visual.transform.localScale;
        while (t < tiempoAchicar)
        {
            t += Time.deltaTime;
            visual.transform.localScale = Vector3.Lerp(inicio, Vector3.zero, t / tiempoAchicar);
            yield return null;
        }
        visual.transform.localScale = Vector3.zero;
        visual.SetActive(false);
    }

    public bool EstaActivo() => _activo;
}
