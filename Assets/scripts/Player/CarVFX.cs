using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(TopDownCarController))]
public class CarVFX : MonoBehaviour
{
    public ParticleSystem jumpSmokePrefab; // Prefab del efecto, no lo adjuntes directamente al auto
    private TopDownCarController car;

    void Awake()
    {
        car = GetComponent<TopDownCarController>();

        // Suscribirse a eventos
        car.onJump.AddListener(OnJump);
        car.onLand.AddListener(OnLand);
    }

    void OnDestroy()
    {
        if (!car) return;
        car.onJump.RemoveListener(OnJump);
        car.onLand.RemoveListener(OnLand);
    }

    void OnJump()
    {
        if (!jumpSmokePrefab) return;

        // Calcular la rotación final
        // Toma la rotación del prefab y reemplaza solo el eje X con el del auto
        Quaternion finalRotation = car.Get_Rotation();
        /*
        Vector3 prefabEuler = finalRotation.eulerAngles;
        prefabEuler.x = transform.eulerAngles.x;
        finalRotation = Quaternion.Euler(prefabEuler);
        */
        // Instanciar una copia en la posición del auto
        ParticleSystem jumpSmoke = Instantiate(
            jumpSmokePrefab,
            transform.position + Vector3.up * 0.2f, // pequeño offset
            finalRotation
        );

        // Reproducir el sistema de partículas
        jumpSmoke.Play();

        // Destruir automáticamente cuando termine
        Destroy(jumpSmoke.gameObject, jumpSmoke.main.duration + jumpSmoke.main.startLifetime.constantMax);
    }

    void OnLand()
    {
        /*
        if (!jumpSmoke) return;

        // Cortar emisión al aterrizar
        var em = jumpSmoke.emission;
        em.enabled = false;
        */
    }
}