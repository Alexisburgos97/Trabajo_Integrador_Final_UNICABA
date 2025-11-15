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

        Vector3 spawnPos = transform.position;

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out var hit, 3f))
            spawnPos = hit.point + Vector3.up * 0.1f;

        Quaternion finalRotation = car.Get_Rotation();
        
        ParticleSystem jumpSmoke = Instantiate(
            jumpSmokePrefab,
            spawnPos,
            finalRotation
        );

        jumpSmoke.Play();

        Destroy(jumpSmoke.gameObject,
            jumpSmoke.main.duration + jumpSmoke.main.startLifetime.constantMax);
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