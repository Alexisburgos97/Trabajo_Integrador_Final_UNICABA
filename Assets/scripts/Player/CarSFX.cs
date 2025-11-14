using UnityEngine;

[RequireComponent(typeof(TopDownCarController))]
[RequireComponent(typeof(AudioSource))]
public class CarSFX : MonoBehaviour
{
    [Header("Clips")]
    public AudioClip jumpClip;
    public AudioClip landClip;

    [Header("Volumen / Pitch")]
    [Range(0,1)] public float jumpVolume = 1f;
    [Range(0,1)] public float landVolume = 0.8f;
    public Vector2 jumpPitchRange = new Vector2(0.95f, 1.08f);
    public Vector2 landPitchRange = new Vector2(0.95f, 1.05f);

    TopDownCarController car;
    AudioSource src;
    
    void Start()
    {
        car = GetComponent<TopDownCarController>();
        src = GetComponent<AudioSource>();

        src.playOnAwake = false;
        src.loop = false;
        src.spatialBlend = 1f;   // 3D
        src.minDistance = 6f;
        src.maxDistance = 40f;
        src.dopplerLevel = 0f;

        car.onJump.AddListener(PlayJump);
        car.onLand.AddListener(PlayLand);
    }

    void OnDestroy()
    {
        if (car != null)
        {
            car.onJump.RemoveListener(PlayJump);
            car.onLand.RemoveListener(PlayLand);
        }
    }

    void PlayJump()
    {
        if (!jumpClip) return;
        src.pitch = Random.Range(jumpPitchRange.x, jumpPitchRange.y);
        src.PlayOneShot(jumpClip, jumpVolume);
    }

    void PlayLand()
    {
        if (!landClip) return;
        src.pitch = Random.Range(landPitchRange.x, landPitchRange.y);
        src.PlayOneShot(landClip, landVolume);
    }
}