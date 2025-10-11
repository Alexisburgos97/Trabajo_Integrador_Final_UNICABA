using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CarEngineSound : MonoBehaviour
{
    [Header("Referencia al Auto")]
    public TopDownCarController carController; // asignalo desde el inspector

    [Header("Pitch")]
    [SerializeField] float minPitch = 0.8f;  // tono al estar quieto
    [SerializeField] float maxPitch = 2.0f;  // tono al máximo de velocidad
    [SerializeField] float pitchLerpSpeed = 5f; // suavizado

    [Header("Volumen")]
    [SerializeField] float minVolume = 0.2f;
    [SerializeField] float maxVolume = 1f;

    [SerializeField] AudioSource engineAudio;
    
    /*
    void Awake()
    {
        engineAudio = GetComponent<AudioSource>();
    }
    */
    void Update()
    {
        if (carController == null) return;

        // Obtener velocidad actual del Rigidbody
        float speed = carController.GetComponent<Rigidbody>().linearVelocity.magnitude;
        float maxSpeed = carController.GetMaxSpeed(); // método que agregaremos

        // Normalizar velocidad 0..1
        float speed01 = Mathf.InverseLerp(0f, maxSpeed, speed);

        // Calcular pitch y volumen deseado
        float targetPitch = Mathf.Lerp(minPitch, maxPitch, speed01);
        float targetVolume = Mathf.Lerp(minVolume, maxVolume, speed01);

        // Suavizar
        engineAudio.pitch = Mathf.Lerp(engineAudio.pitch, targetPitch, Time.deltaTime * pitchLerpSpeed);
        engineAudio.volume = Mathf.Lerp(engineAudio.volume, targetVolume, Time.deltaTime * pitchLerpSpeed);
    }
}
