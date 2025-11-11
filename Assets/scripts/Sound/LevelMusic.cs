using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    [SerializeField] private AudioClip music;
    [SerializeField] private GameObject persistentSoundPrefab;

    void Start()
    {
        if (!PersistentSound.Exists && persistentSoundPrefab != null)
            Instantiate(persistentSoundPrefab);

        PersistentSound.PlayMusic(music);
    }
}