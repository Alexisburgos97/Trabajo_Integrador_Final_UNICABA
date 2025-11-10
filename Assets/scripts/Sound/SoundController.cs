using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour, ISoundController
{
    [SerializeField] private AudioSource audioSourceSFXOnce;
    [SerializeField] private AudioSource audioSourceSFXLoop;
    [SerializeField] private AudioSource audioSourceMusicLoop;

    void Start()
    {
        //SFX
        if (audioSourceSFXOnce != null)
        {
            audioSourceSFXOnce.volume = PlayerPrefs.GetFloat("SFX", 0.3f);
        }
        //--LOOP SFX (Esto no se usa ni en pedo)
        if (audioSourceSFXLoop != null)
        {
            audioSourceSFXLoop.volume = PlayerPrefs.GetFloat("SFX", 0.3f);
        }

        //MUSIC
        if (audioSourceMusicLoop != null)
        {
            audioSourceMusicLoop.volume = PlayerPrefs.GetFloat("Music", 0.3f);
        }
    }

    private void Update()
    {
        /*
        audioSourceSFXOnce.volume = PlayerPrefs.GetFloat("SFX", 0.3f);
        audioSourceSFXLoop.volume = PlayerPrefs.GetFloat("SFX", 0.3f);
        audiosourceMusic.volume = PlayerPrefs.GetFloat("Music", 0.3f);
        audioSourceMusicLoop.volume = PlayerPrefs.GetFloat("Music", 0.3f);*/
    }

    public void PlaySFX(AudioClip clip)
    {
        audioSourceSFXOnce.loop = false;
        audioSourceSFXOnce.PlayOneShot(clip);
    }

    public void PlaySFXLoop(AudioClip clip)
    {
        audioSourceSFXLoop.loop = true;
        audioSourceSFXLoop.clip = clip;
        audioSourceSFXLoop.Play();
    }

    public void PlayMusic(AudioClip clip)
    {
        audioSourceMusicLoop.loop = true;
        audioSourceMusicLoop.clip = clip;
        audioSourceMusicLoop.Play();
    }

    public void StopLoopSound()
    {
        audioSourceMusicLoop.Stop();
        audioSourceSFXLoop.Stop();
    }
}
