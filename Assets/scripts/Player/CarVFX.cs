using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TopDownCarController))]
public class CarVFX : MonoBehaviour
{
    public ParticleSystem jumpSmokePrefab;

    [Header("Raycast para suelo")]
    [SerializeField] float rayLen = 5f;
    [SerializeField] float groundOffset = 0.22f;

    private TopDownCarController car;

    void Awake()
    {
        car = GetComponent<TopDownCarController>();
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

        // 1) RaycastAll para detectar TODOS los colliders debajo del auto
        Vector3 origin = transform.position + Vector3.up * 1.0f;
        var hits = Physics.RaycastAll(origin, Vector3.down, rayLen, ~0, QueryTriggerInteraction.Ignore);

        if (hits.Length == 0)
        {
            Debug.LogWarning("[JumpSmoke] No se detectó suelo debajo del auto.");
            return;
        }

        // 2) Tomar el hit con MAYOR altura (más alto en Y)
        var hit = hits.Aggregate((a, b) => a.point.y > b.point.y ? a : b);

        // 3) Calcular posición y rotación del humo
        Vector3 spawnPos = hit.point + hit.normal * groundOffset;
        Vector3 fwd = Vector3.ProjectOnPlane(transform.forward, hit.normal);
        if (fwd.sqrMagnitude < 1e-4f) fwd = Vector3.forward;
        Quaternion spawnRot = Quaternion.LookRotation(fwd.normalized, hit.normal);

        // 4) Instanciar el sistema de partículas
        var ps = Instantiate(jumpSmokePrefab, spawnPos, spawnRot);

        // Simular en espacio mundial para que no siga al auto
        var main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        // Subir prioridad visual
        var rend = ps.GetComponent<ParticleSystemRenderer>();
        if (rend)
        {
            rend.sortingFudge = 10f;
            if (rend.material != null)
                rend.material.renderQueue = 3100; // Transparent+100
        }

        // Emitir inmediatamente
        var em = ps.emission; em.enabled = true;
        ps.Clear(true);
        ps.Emit(30);
        ps.Play();

        Destroy(ps.gameObject, main.duration + main.startLifetime.constantMax + 0.5f);
    }

    void OnLand() { }
}
