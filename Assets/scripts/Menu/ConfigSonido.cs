using UnityEngine;
using UnityEngine.UI;

public class ConfigSonido : MonoBehaviour
{
    [SerializeField] private Slider MusicVolumeSlider;
    [SerializeField] private Slider SFXVolumeSlider;

    private void Awake()
    {
        MusicVolumeSlider.onValueChanged.AddListener(SetMusicVolumen);
        SFXVolumeSlider.onValueChanged.AddListener(SetSFXVolumen);
    }

    private void Start()
    {
        MusicVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MusicVolume", 0.3f));
        SFXVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFXVolume", 0.3f));
    }

    private void SetMusicVolumen(float NewVolume)
    {
        PlayerPrefs.SetFloat("MusicVolume", NewVolume);
        PlayerPrefs.Save();
    }

    private void SetSFXVolumen(float NewVolume)
    {
        PlayerPrefs.SetFloat("SFXVolume", NewVolume);
        PlayerPrefs.Save();
    }
}