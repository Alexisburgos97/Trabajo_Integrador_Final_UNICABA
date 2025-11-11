using UnityEngine;

[RequireComponent(typeof(SoundController))]
public class PersistentSound : MonoBehaviour
{
    private static PersistentSound instance;
    private SoundController soundController;

    [SerializeField] private AudioClip initialMusic;

    public static bool Exists => instance != null;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        soundController = GetComponent<SoundController>();
        if (soundController != null && initialMusic != null)
            soundController.PlayMusic(initialMusic);
    }

    /// <summary>
    /// Con esto cambias la música
    /// </summary>
    public static void PlayMusic(AudioClip clip)
    {
        if (instance == null || instance.soundController == null || clip == null)
            return;

        instance.soundController.PlayMusic(clip);
    }

    /// <summary>
    /// Matar al componente capo, por si necesitas parar la música
    /// </summary>
    public static void Kill()
    {
        if (instance == null) return;
        Destroy(instance.gameObject);
        instance = null;
    }
}