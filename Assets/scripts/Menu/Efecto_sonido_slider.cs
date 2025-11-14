using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Efecto_sonido_slider : MonoBehaviour
{
    [Header("Fuentes de Audio")]
    [SerializeField] private AudioSource hoverAudioSource;     // Sonido al pasar el mouse
    [SerializeField] private AudioSource sliderAudioSource;    // Sonido al mover el slider

    [Header("Sliders a controlar")]
    [SerializeField] private List<Slider> sliders = new List<Slider>();

    [Header("Configuración del efecto de cuerda")]
    [Tooltip("Tiempo mínimo (en segundos) entre reproducciones del sonido de cuerda.")]
    [SerializeField] private float intervaloEntreSonidos = 0.1f;

    private bool puedeReproducir = true;

    void Start()
    {
        foreach (Slider slider in sliders)
        {
            if (slider == null) continue;

            // 🎚️ Agregar evento para reproducir sonido de cuerda al mover el slider
            slider.onValueChanged.AddListener((value) => OnSliderMoved());

            // 🖱️ Agregar evento para reproducir sonido hover al pasar el mouse
            EventTrigger trigger = slider.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = slider.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entryHover = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entryHover.callback.AddListener((data) =>
            {
                if (hoverAudioSource != null)
                    hoverAudioSource.Play();
            });
            trigger.triggers.Add(entryHover);
        }
    }

    private void OnSliderMoved()
    {
        if (sliderAudioSource == null || !puedeReproducir)
            return;

        // Evitar solapamiento de sonidos
        if (!sliderAudioSource.isPlaying)
        {
            sliderAudioSource.Play();
            StartCoroutine(EsperarIntervalo());
        }
    }

    private IEnumerator EsperarIntervalo()
    {
        puedeReproducir = false;

        // Esperar la duración del clip más el intervalo adicional configurable
        float duracion = sliderAudioSource.clip != null ? sliderAudioSource.clip.length : 0f;
        yield return new WaitForSeconds(duracion + intervaloEntreSonidos);

        puedeReproducir = true;
    }
}
