using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DriftVFXAuto : MonoBehaviour
{
    [Header("Referencias")]
    public TopDownCarController car;          // opcional
    public LayerMask groundMask = ~0;         // p.ej. "Caminos"
    public Material skidMaterial;
    public Material smokeMaterial;
    public AudioClip skidClip;
    [Range(0, 1)] public float audioMaxVol = 0.9f;

    [Header("Activación adicional")]
    public bool triggerOnHandbrake = true;
    public KeyCode handbrakeKey = KeyCode.Space;

    [Header("Tuning")]
    public float minSpeed = 6f;
    public float slipThreshold = 0.6f;
    public float slipForMax = 1.8f;
    public float rayLen = 2f;

    [Header("Wheel Prefabs (opcional)")]
    public GameObject wheelRL_Prefab;
    public GameObject wheelRR_Prefab;

    // Runtime
    Rigidbody rb;
    Transform pRL, pRR;
    Vector3 rlLocal, rrLocal;   // offsets usados SOLO si no hay prefabs
    TrailRenderer trRL, trRR;
    ParticleSystem psRL, psRR;
    AudioSource audioSrc;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!car) car = GetComponent<TopDownCarController>();

        bool haveRL = wheelRL_Prefab != null;
        bool haveRR = wheelRR_Prefab != null;

        if (!haveRL || !haveRR)
        {
            // calcular offsets por bounds solo si faltan prefabs
            Bounds b = CalcWorldBounds();
            float halfW = b.extents.x;
            float halfL = b.extents.z;

            const float trackFactor = 0.80f;
            const float axleFactor  = 0.90f;
            const float yLocal      = -0.05f;

            rlLocal = new Vector3(-halfW * trackFactor, yLocal, -halfL * axleFactor);
            rrLocal = new Vector3(+halfW * trackFactor, yLocal, -halfL * axleFactor);
        }

        // Crear/ubicar puntos:
        if (haveRL)
        {
            var rl = Instantiate(wheelRL_Prefab, transform);
            rl.transform.localPosition = wheelRL_Prefab.transform.localPosition;
            pRL = rl.transform;
            rlLocal = pRL.localPosition;
        }
        else
        {
            pRL = CreatePoint("FX_RL", rlLocal);
        }

        if (haveRR)
        {
            var rr = Instantiate(wheelRR_Prefab, transform);
            rr.transform.localPosition = wheelRR_Prefab.transform.localPosition;
            pRR = rr.transform;
            rrLocal = pRR.localPosition;
        }
        else
        {
            pRR = CreatePoint("FX_RR", rrLocal);
        }
        
        rlLocal = pRL.localPosition;
        rrLocal = pRR.localPosition;

        // Efectos
        trRL = CreateTrail(pRL);   trRR = CreateTrail(pRR);
        psRL = CreateSmoke(pRL);   psRR = CreateSmoke(pRR);

        // Audio
        audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.clip = skidClip;
        audioSrc.loop = true;
        audioSrc.playOnAwake = false;
        audioSrc.spatialBlend = 1f;
        audioSrc.minDistance = 6f;
        audioSrc.maxDistance = 40f;
    }
    
    void LateUpdate()
    {
        if (pRL)
        {
            pRL.localPosition = rlLocal;
            pRL.localRotation = Quaternion.identity;
        }
        if (pRR)
        {
            pRR.localPosition = rrLocal;
            pRR.localRotation = Quaternion.identity;
        }
    }

    Bounds CalcWorldBounds()
    {
        var rends = GetComponentsInChildren<Renderer>();
        if (rends.Length == 0)
        {
            var cols = GetComponentsInChildren<Collider>();
            Bounds bb = new Bounds(transform.position, Vector3.one);
            foreach (var c in cols) bb.Encapsulate(c.bounds);
            return bb;
        }
        Bounds b = rends[0].bounds;
        foreach (var r in rends) b.Encapsulate(r.bounds);
        return b;
    }

    Transform CreatePoint(string name, Vector3 localOffset)
    {
        var t = new GameObject(name).transform;
        t.SetParent(transform, false);
        t.localPosition = localOffset;
        return t;
    }

    TrailRenderer CreateTrail(Transform parent)
    {
        var tr = parent.gameObject.AddComponent<TrailRenderer>();
        tr.time = 2.5f;
        tr.minVertexDistance = 0.06f;
        tr.widthCurve = AnimationCurve.Linear(0, 0.18f, 1, 0f);
        tr.material = skidMaterial;
        tr.textureMode = LineTextureMode.Tile;

        var g = new Gradient();
        g.SetKeys(
            new[] {
                new GradientColorKey(Color.black, 0f),
                new GradientColorKey(Color.black, 0.7f),
                new GradientColorKey(Color.black, 1f)
            },
            new[] {
                new GradientAlphaKey(0.75f, 0f),
                new GradientAlphaKey(0.75f, 0.7f),
                new GradientAlphaKey(0.0f, 1f)
            }
        );
        tr.colorGradient = g;

        tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        tr.receiveShadows = false;
        tr.emitting = false;
        return tr;
    }

    ParticleSystem CreateSmoke(Transform parent)
    {
        var ps = parent.gameObject.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = 1.0f;
        main.startSpeed    = 0.8f;
        main.startSize     = 0.55f;
        main.startColor    = new Color(0.5f, 0.5f, 0.5f, 0.7f);
        main.playOnAwake   = false;

        var em = ps.emission; em.enabled = false; em.rateOverTime = 0f;

        var shape = ps.shape; shape.shapeType = ParticleSystemShapeType.Cone; shape.angle = 10f; shape.radius = 0.06f;

        var rend = ps.GetComponent<ParticleSystemRenderer>();
        if (smokeMaterial) rend.material = smokeMaterial;
        rend.sortingFudge = 2f;

        var col = ps.colorOverLifetime; col.enabled = true;
        var g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(Color.gray, 0f), new GradientColorKey(Color.gray, 1f) },
            new[] { new GradientAlphaKey(0.8f, 0f),      new GradientAlphaKey(0f,   1f) }
        );
        col.color = new ParticleSystem.MinMaxGradient(g);

        return ps;
    }

    void Update()
    {
        float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude; // <- velocity
        float slip = ComputeSlip();

        bool drifting = false;
        if (car != null) drifting |= car.IsDrifting;
        drifting |= speed > minSpeed && slip > slipThreshold;
        if (triggerOnHandbrake && Input.GetKey(handbrakeKey))
            drifting |= speed > (minSpeed * 0.4f);

        UpdateWheelFXAt(pRL, trRL, psRL, drifting, slip);
        UpdateWheelFXAt(pRR, trRR, psRR, drifting, slip);
        UpdateAudio(drifting, slip);
    }

    float ComputeSlip()
    {
        Vector3 local = transform.InverseTransformDirection(rb.linearVelocity);
        return Mathf.Abs(local.x) * 0.15f;
    }
    
    void UpdateWheelFXAt(Transform anchor, TrailRenderer tr, ParticleSystem ps, bool active, float slip)
    {
        if (!anchor) return;

        // --- usar mundo, no el "up" del auto ---
        Vector3 rayOrigin = anchor.position + Vector3.up * 0.5f; // subir un poco en MUNDO
        Vector3 rayDir    = Vector3.down;                        // bajar en MUNDO

        // más tolerante que Raycast cuando hay pequeñas inclinaciones
        bool grounded = Physics.SphereCast(rayOrigin, 0.15f, rayDir, out var hit, rayLen,
            groundMask, QueryTriggerInteraction.Ignore);

        // trail pegado al anchor
        if (tr)
        {
            tr.transform.position = anchor.position;
            tr.emitting = active && grounded;
        }

        if (ps)
        {
            bool shouldEmit = active && grounded;
            var em = ps.emission;
            em.enabled = shouldEmit;

            if (shouldEmit && !ps.isPlaying) ps.Play();
            else if (!shouldEmit && ps.isPlaying) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            if (grounded)
            {
                // colocar justo debajo de la rueda
                Vector3 targetPos = anchor.position;
                targetPos.y = hit.point.y + 0.06f;
                ps.transform.position = targetPos;

                ps.transform.rotation = Quaternion.LookRotation(
                    Vector3.ProjectOnPlane(transform.forward, hit.normal), hit.normal);

                float t = Mathf.InverseLerp(slipThreshold, slipForMax, slip);
                var rate = em.rateOverTime; rate.constant = Mathf.Lerp(25f, 90f, t); em.rateOverTime = rate;
            }
        }
    }
    
    void UpdateAudio(bool active, float slip)
    {
        if (!audioSrc || !skidClip) return;
        float target = active ? Mathf.InverseLerp(slipThreshold, slipForMax, slip) * audioMaxVol : 0f;
        audioSrc.volume = Mathf.MoveTowards(audioSrc.volume, target, Time.deltaTime * 3f);
        audioSrc.pitch  = 0.9f + 0.25f * (audioSrc.volume / Mathf.Max(0.001f, audioMaxVol));
        if (audioSrc.volume > 0.01f && !audioSrc.isPlaying) audioSrc.Play();
        if (audioSrc.volume <= 0.01f && audioSrc.isPlaying) audioSrc.Stop();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        if (pRL) Gizmos.DrawSphere(pRL.position, 0.06f);
        if (pRR) Gizmos.DrawSphere(pRR.position, 0.06f);
    }
#endif
    
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (wheelRL_Prefab) Gizmos.DrawSphere(wheelRL_Prefab.transform.position, 0.1f);
        if (wheelRR_Prefab) Gizmos.DrawSphere(wheelRR_Prefab.transform.position, 0.1f);
    }
#endif
}