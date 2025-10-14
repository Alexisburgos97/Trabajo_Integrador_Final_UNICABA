using UnityEngine;

[RequireComponent(typeof(TopDownCarController))]
public class CarVFX : MonoBehaviour
{
    public ParticleSystem jumpSmoke;

    TopDownCarController car;

    void Awake()
    {
        car = GetComponent<TopDownCarController>();

        // Fuerza PlayOnAwake = false y Looping = false al iniciar
        if (jumpSmoke)
        {
            var main = jumpSmoke.main;
            main.playOnAwake = false;
            main.loop = false;
            jumpSmoke.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        car.onJump.AddListener(OnJump);
        car.onLand.AddListener(OnLand);
    }

    void OnDestroy()
    {
        if (car)
        {
            car.onJump.RemoveListener(OnJump);
            car.onLand.RemoveListener(OnLand);
        }
    }

    void OnJump()
    {
        if (!jumpSmoke) return;
        jumpSmoke.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        jumpSmoke.Play();
    }

    void OnLand()
    {
        // Si alguna vez lo pones en loop, lo apagas al aterrizar
        if (!jumpSmoke) return;
        jumpSmoke.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}