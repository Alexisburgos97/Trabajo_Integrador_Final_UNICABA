using UnityEngine;

[RequireComponent(typeof(TopDownCarController))]
public class CarVFX : MonoBehaviour
{
    public ParticleSystem jumpSmoke;

    TopDownCarController car;

    void Awake()
    {
        car = GetComponent<TopDownCarController>();

        if (jumpSmoke)
        {
            var main = jumpSmoke.main;
            main.playOnAwake = false;
            main.loop = false;
            jumpSmoke.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

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
        if (!jumpSmoke) return;

        // Forzar que emita aunque el Emission rate sea 0
        var em = jumpSmoke.emission;
        em.enabled = true;

        // Opcional: centrar el humo en el auto (útil si el prefab quedó medio corrido)
        jumpSmoke.transform.position = transform.position + Vector3.down * 0.1f;

        // Reproducir y emitir un burst
        jumpSmoke.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        jumpSmoke.Play();
        jumpSmoke.Emit(35);   // cantidad de partículas del “puff”
    }

    void OnLand()
    {
        if (!jumpSmoke) return;

        // Cortar emisión al aterrizar
        var em = jumpSmoke.emission;
        em.enabled = false;
    }
}