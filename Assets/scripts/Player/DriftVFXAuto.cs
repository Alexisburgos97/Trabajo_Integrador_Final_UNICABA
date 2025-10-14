using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DriftVFXAuto : MonoBehaviour
{
    [Header("Referencias")]
    public TopDownCarController car;          // opcional (solo para leer IsDrifting)
    public LayerMask groundMask = ~0;         // capas de suelo (p.ej. Caminos)
    public Material skidMaterial;             // material Unlit/Transparent NEGRO
    public Material smokeMaterial;            // material Particles/Unlit gris
    public AudioClip skidClip;                // audio chillido (loop)
    [Range(0, 1)] public float audioMaxVol = 0.9f;

    [Header("Activación adicional")]
    public bool triggerOnHandbrake = true;    // también activar VFX con el freno de mano
    public KeyCode handbrakeKey = KeyCode.Space;

    [Header("Tuning")]
    public float minSpeed = 6f;               // velocidad mínima para VFX si no hay IsDrifting
    public float slipThreshold = 0.6f;        // slip para empezar humo
    public float slipForMax = 1.8f;           // slip para humo máximo
    public float rayLen = 2f;                 // alcance raycast a suelo
    
    public Transform wheelRLAnchor;
    public Transform wheelRRAnchor;

    // Runtime
    Rigidbody rb;

    // Puntos “virtuales” de ruedas traseras (solo para tener los componentes)
    Transform pRL, pRR;
    // Offsets LOCALES fijos respecto al auto (la clave para que no se desalineen)
    Vector3 rlLocal, rrLocal;

    TrailRenderer trRL, trRR;
    ParticleSystem psRL, psRR;
    AudioSource audioSrc;

    void Awake()
    {
        // rb = GetComponent<Rigidbody>();
        // if (!car) car = GetComponent<TopDownCarController>();
        //
        // // --- calcular tamaño del coche para ubicar las ruedas traseras ---
        // Bounds b = CalcWorldBounds();
        // float halfW = b.extents.x;
        // float halfL = b.extents.z;
        //
        // const float trackFactor = 0.80f; // qué tan afuera las ruedas
        // const float axleFactor  = 0.90f; // qué tan atrás el eje
        // const float yLocal      = -0.05f; // pequeño ajuste vertical local
        //
        // rlLocal = new Vector3(-halfW * trackFactor, yLocal, -halfL * axleFactor);
        // rrLocal = new Vector3(+halfW * trackFactor, yLocal, -halfL * axleFactor);
        //
        // // Crear puntos hijos (solo contenedores de Trail/PS)
        // pRL = CreatePoint("FX_RL", rlLocal);
        // pRR = CreatePoint("FX_RR", rrLocal);
        //
        // // Crear componentes de marcas y humo
        // trRL = CreateTrail(pRL);   trRR = CreateTrail(pRR);
        // psRL = CreateSmoke(pRL);   psRR = CreateSmoke(pRR);
        
        rb = GetComponent<Rigidbody>();
        if (!car) car = GetComponent<TopDownCarController>();

        // Si hay anclas, uso sus posiciones locales; si no, calculo por bounds.
        if (wheelRLAnchor && wheelRRAnchor)
        {
            rlLocal = wheelRLAnchor.localPosition;
            rrLocal = wheelRRAnchor.localPosition;
        }
        else
        {
            Bounds b = CalcWorldBoundsOnlyMeshes();
            float halfW = b.extents.x;
            float halfL = b.extents.z;

            const float trackFactor = 0.80f;
            const float axleFactor  = 0.90f;
            const float yLocal      = -0.05f;

            rlLocal = new Vector3(-halfW * trackFactor, yLocal, -halfL * axleFactor);
            rrLocal = new Vector3(+halfW * trackFactor, yLocal, -halfL * axleFactor);
        }

        pRL = CreatePoint("FX_RL", rlLocal);
        pRR = CreatePoint("FX_RR", rrLocal);

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
    
    Bounds CalcWorldBoundsOnlyMeshes()
    {
        var meshRends = GetComponentsInChildren<MeshRenderer>();
        var skinned   = GetComponentsInChildren<SkinnedMeshRenderer>();

        Bounds? opt = null;
        foreach (var r in meshRends)
            opt = opt.HasValue ? Enc(opt.Value, r.bounds) : r.bounds;
        foreach (var r in skinned)
            opt = opt.HasValue ? Enc(opt.Value, r.bounds) : r.bounds;

        return opt ?? new Bounds(transform.position, Vector3.one);

        static Bounds Enc(Bounds a, Bounds b) { a.Encapsulate(b); return a; }
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
        t.localPosition = localOffset; // ¡no se vuelve a tocar!
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

        var em = ps.emission;
        em.enabled = false;
        em.rateOverTime = new ParticleSystem.MinMaxCurve(0f);

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle     = 10f;
        shape.radius    = 0.06f;

        var rend = ps.GetComponent<ParticleSystemRenderer>();
        if (smokeMaterial) rend.material = smokeMaterial;
        rend.sortingFudge = 2f;

        var col = ps.colorOverLifetime;
        col.enabled = true;
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
        float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
        float slip = ComputeSlip();

        bool drifting = false;
        if (car != null) drifting |= car.IsDrifting;                       // si el controller lo informa
        drifting |= (speed > minSpeed && slip > slipThreshold);            // heurística por velocidad/lateral
        if (triggerOnHandbrake && Input.GetKey(handbrakeKey))              // también con freno de mano
            drifting |= speed > (minSpeed * 0.4f);                         // permite algo más lento

        UpdateWheelFXLocal(rlLocal, trRL, psRL, drifting, slip);
        UpdateWheelFXLocal(rrLocal, trRR, psRR, drifting, slip);
        UpdateAudio(drifting, slip);
    }

    float ComputeSlip()
    {
        Vector3 local = transform.InverseTransformDirection(rb.linearVelocity);
        return Mathf.Abs(local.x) * 0.15f;
    }

    void UpdateWheelFXLocal(Vector3 localOffset, TrailRenderer tr, ParticleSystem ps, bool active, float slip)
    {
        // Posición mundial ideal de la rueda a partir del offset local
        Vector3 wheelBase = transform.TransformPoint(localOffset);

        // Raycast hacia abajo (o “up” del coche para rampas)
        Vector3 upDir = transform.up;
        bool grounded = Physics.Raycast(wheelBase + upDir * 0.25f, -upDir,
                                        out var hit, rayLen, groundMask,
                                        QueryTriggerInteraction.Ignore);

        // TRAIL
        if (tr)
        {
            if (grounded) tr.transform.position = hit.point + hit.normal * 0.05f;
            tr.emitting = active && grounded;
        }

        // HUMO
        if (ps)
        {
            bool shouldEmit = active && grounded;

            var em = ps.emission;
            em.enabled = shouldEmit;

            if (shouldEmit && !ps.isPlaying) ps.Play();
            if (!shouldEmit && ps.isPlaying) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            if (grounded)
            {
                ps.transform.position = hit.point + hit.normal * 0.06f;
                ps.transform.rotation = Quaternion.LookRotation(
                    Vector3.ProjectOnPlane(transform.forward, hit.normal), hit.normal);

                float t = Mathf.InverseLerp(slipThreshold, slipForMax, slip);
                var rate = em.rateOverTime; rate.constant = Mathf.Lerp(25f, 90f, t);
                em.rateOverTime = rate;
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
        // Gizmos para ver dónde se raycastea (posiciones locales transformadas)
        Gizmos.color = Color.magenta;
        if (pRL) Gizmos.DrawSphere(transform.TransformPoint(rlLocal), 0.06f);
        if (pRR) Gizmos.DrawSphere(transform.TransformPoint(rrLocal), 0.06f);
    }
#endif
}
