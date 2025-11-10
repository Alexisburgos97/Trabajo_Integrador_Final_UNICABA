using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIButtonSoundController : MonoBehaviour
{
    [Header("Fuentes de Audio")]
    [SerializeField] private AudioSource hoverAudioSource; // Sonido mouse over
    [SerializeField] private AudioSource clickAudioSource; // Sonido click

    [Header("Botones a controlar")]
    [SerializeField] private List<Button> botones = new List<Button>();

    void Start()
    {
        foreach (Button btn in botones)
        {
            if (btn == null) continue;

            // Agregar sonido al hacer click
            btn.onClick.AddListener(() =>
            {
                if (clickAudioSource != null)
                    clickAudioSource.Play();
            });

            // Agregar sonido al pasar el mouse por encima
            EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = btn.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entry.callback.AddListener((data) =>
            {
                if (hoverAudioSource != null)
                    hoverAudioSource.Play();
            });
            trigger.triggers.Add(entry);
        }
    }
}
