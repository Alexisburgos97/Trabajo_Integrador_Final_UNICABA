using UnityEngine;
using System.Collections;

public class EscudoJugador : MonoBehaviour
{
    [Header("Configuraci�n del Escudo")]
    public GameObject visual;           // Escudo visual
    public AudioSource audioActivacion;
    public float duracionEscudo = 5f;
    public float tiempoCrecer = 0.3f;   // tiempo de animaci�n de aparici�n
    public float tiempoAchicar = 0.2f;  // tiempo de animaci�n de desaparici�n
    public float parpadeoAntes = 1.0f;  // segundos de parpadeo al final
    public float frecuenciaParpadeo = 0.1f; // intervalo de parpadeo

    private bool _activo=false;
    private Vector3 _escalaOriginal;
    // private PlayerStats stats;

    void Start()
    {
        // stats = GetComponentInParent<PlayerStats>();
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
        // if (stats) stats.tieneEscudo = true;

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
        if (visual != null)
            StartCoroutine(AchicarYParpadear());
        _activo = false;
        // if (stats) stats.tieneEscudo = false;
    }

    private IEnumerator AchicarYParpadear()
    {
        float tiempoTotal = parpadeoAntes;
        float contador = 0f;
        bool visible = true;

        // Parpadeo
        while (contador < tiempoTotal)
        {
            visible = !visible;
            visual.SetActive(visible);
            contador += frecuenciaParpadeo;
            yield return new WaitForSeconds(frecuenciaParpadeo);
        }

        // Asegurarse de que se vea antes de achicar
        visual.SetActive(true);

        // Achicar
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
