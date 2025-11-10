using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISoundController
{
    void PlaySFX(AudioClip clip);
    void PlayMusic(AudioClip clip);
    void PlaySFXLoop(AudioClip clip);
    void StopLoopSound();
}
