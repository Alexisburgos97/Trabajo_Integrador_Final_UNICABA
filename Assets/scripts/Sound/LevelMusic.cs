using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    //SOUND CONTROLLER
    SoundController soundController;

    [SerializeField] private AudioClip Music;

    private void Start()
    {
        soundController = GetComponent<SoundController>();
        soundController.PlayMusic(Music);
    }
}
