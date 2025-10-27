using UnityEngine;
using System.Collections;
using Simplon;
#if UNITY_VISUAL_EFFECT_GRAPH
using UnityEngine.VFX;
#endif

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Enemy_ProxBomber : MonoBehaviour
{
    [Header("Objetivo")]
    public string targetTag = "Player";
    Transform target;

    [Header("Movimiento")]
    public float moveSpeed = 12f;
    public float turnSpeed = 200f;

    [Header("Evitar superposición")]
    public float separationRadius = 4f;
    public float separationForce = 40f;

    [Header("Proximidad / Detonación")]
    [Tooltip("Cuando el player entra en este radio, el enemigo empieza a TELEGRAPH (aviso).")]
    public float armDistance = 12f;
    [Tooltip("Si el player entra en este radio (o termina el telegraph), detona.")]
    public float explodeDistance = 6f;
    [Tooltip("Duración del aviso antes de detonar (si el player no se aleja).")]
    public float telegraphTime = 5f;

    [Header("Daño / Knockback")]
    public float fuelDrain = 30f;
    public float explosionForce = 30000f;
    public float explosionRadius = 6f;
    public float upwardsModifier = 0f;

    [Header("Aviso (telegraph)")]
    public GameObject warningVfx;   // círculo/halo
    public AudioClip warningBeep;   // beep acelerado
    public Color  flashColor = Color.red;
    public float  flashIntensity = 3.5f;

    [Header("Explosión (feedback)")]
    public GameObject explodeVfx;
    public AudioClip  explodeSfx;
    [Range(0,1)] public float sfxVolume = 1f;
    
    [Header("Ráfaga de proyectiles al morir")]
    public GameObject projectilePrefab;
    public int projectileCount = 3;
    
    [Tooltip("Apertura angular total del abanico (°)")]
    public float projectileCone = 35f;
    
    [Tooltip("Separación vertical del punto de spawn")]
    public float projectileSpawnYOffset = 1.2f;
    
    [Tooltip("Separación lateral entre cada misil (metros)")]
    public float projectileLateralSpacing = 2f;   
    
    [Tooltip("Pequeño offset hacia adelante para que no choquen entre sí")]
    public float projectileForwardOffset = 2f;   
    public float projectileDelayBetween = 0.06f;
    
    [Header("Variaciones de enemigo")]
    public bool useMissileVariation = false;
    
    enum State { Chase, Arming }
    State state = State.Chase;

    Rigidbody rb;
    Material instancedMat;
    AudioSource audioSrc;
    GameControler gc;
    Renderer rend;
    GameObject spawnedWarning;

    [SerializeField] LayerMask groundMask = ~0;
    [SerializeField] float vfxYOffset = 0.2f;  // separación del suelo
    [SerializeField] float vfxLifetime = 3f;   // vida mínima del VFX

    Quaternion warningBaseRot;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        rend = GetComponentInChildren<Renderer>();
        if (rend && rend.material) instancedMat = rend.material; // instancia

        audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        audioSrc.spatialBlend = 1f;
        audioSrc.minDistance = 6f;
        audioSrc.maxDistance = 40f;
    }

    void Start()
    {
        gc = GameControler.Instance;

        var tObj = GameObject.FindGameObjectWithTag(targetTag);
        if (tObj) target = tObj.transform;
        else Debug.LogWarning("Enemy_ProxBomber: no se encontró target con tag " + targetTag);
    }

    void FixedUpdate()
    {
        if (!target) return;

        float dist = Vector3.Distance(transform.position, target.position);

        switch (state)
        {
            case State.Chase:
                if (dist <= armDistance)
                {
                    StartCoroutine(TelegraphAndMaybeExplode());
                    state = State.Arming;
                }
                else
                {
                    MoveTick();
                }
                break;

            case State.Arming:
                // el movimiento reducido lo maneja la coroutine
                break;
        }
    }

    // ====== TELEGRAPH + EXPLODE (coroutine con yields) ======
IEnumerator TelegraphAndMaybeExplode()
{
    if (warningVfx && !spawnedWarning)
    {
        Vector3 p = transform.position + Vector3.up * 0.05f;
        warningBaseRot = warningVfx.transform.rotation;

        spawnedWarning = Instantiate(warningVfx, p, warningBaseRot, transform);

        foreach (var ps in spawnedWarning.GetComponentsInChildren<ParticleSystem>(true))
        {
            
            var main = ps.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.startSpeed      = 0f;
            main.playOnAwake     = true;
            main.cullingMode     = ParticleSystemCullingMode.AlwaysSimulate; // 👈

            var em = ps.emission; em.enabled = true;

            ps.Clear(true);
            ps.Play(true);
        }

        // dejar que arranque el sistema
        yield return null;

        // blindaje contra “nació en 0”
        StartCoroutine(VerifyAndKickParticles(spawnedWarning)); // 👈
        StartCoroutine(StickToGround(spawnedWarning.transform));
    }

    // 2) Aviso con tiempo mínimo
    const float minTelegraphTime = 0.35f; // puedes exponerlo si quieres
    float t = 0f;
    float nextBeep = 0f;
    bool quiereExplotar = false;

    while (t < telegraphTime)
    {
        // si ya está dentro del radio de explosión, marca para explotar ASAP
        if (Vector3.Distance(transform.position, target.position) <= explodeDistance)
            quiereExplotar = true;

        // movimiento lento + feedback
        MoveTick(0.6f);

        if (instancedMat && instancedMat.HasProperty("_EmissionColor"))
        {
            float k = Mathf.PingPong(Time.time * 6f, 1f);
            Color baseCol = flashColor * (k * flashIntensity);
            instancedMat.SetColor("_EmissionColor", baseCol);
            DynamicGI.SetEmissive(rend, baseCol);
        }

        if (warningBeep && Time.time >= nextBeep)
        {
            audioSrc.pitch = Mathf.Lerp(0.9f, 1.6f, t / telegraphTime);
            audioSrc.volume = 0.85f;
            audioSrc.PlayOneShot(warningBeep, 1f);
            float interval = Mathf.Lerp(0.35f, 0.08f, t / telegraphTime);
            nextBeep = Time.time + interval;
        }

        t += Time.fixedDeltaTime;
        yield return new WaitForFixedUpdate();

        // deja que el warning viva aunque sea minTelegraphTime
        if (quiereExplotar && t >= minTelegraphTime) break;
    }

    if (spawnedWarning) Destroy(spawnedWarning);
    ExplodeNow();
}

    // Mantiene el warning pegado al piso mientras exista
    IEnumerator StickToGround(Transform fx)
    {
        while (fx && spawnedWarning == fx.gameObject)
        {
            Vector3 origin = transform.position + Vector3.up * 1f;

            if (Physics.Raycast(origin, Vector3.down, out var hit, 5f, groundMask, QueryTriggerInteraction.Ignore))
            {
                fx.position = hit.point + hit.normal * 0.15f;

                // alinear el "up" al normal del piso, PERO preservando la rotación base (90° en X)
                Quaternion alignUpToGround = Quaternion.FromToRotation(Vector3.up, hit.normal);
                fx.rotation = alignUpToGround * warningBaseRot;
            }
            else
            {
                fx.position = transform.position + Vector3.up * 0.05f;
                fx.rotation = warningBaseRot;
            }

            yield return null;
        }
    }

    // ====== EXPLOSIÓN ======
    void ExplodeNow()
    {
        // posición en el piso
        Vector3 p = transform.position + Vector3.up * 1.5f;
        if (Physics.Raycast(p, Vector3.down, out var hit, 5f, groundMask, QueryTriggerInteraction.Ignore))
            p = hit.point + hit.normal * vfxYOffset;
        else
            p = transform.position + Vector3.up * vfxYOffset;

        // VFX (una sola instancia, protegida)
        if (explodeVfx)
        {
            var go = Instantiate(explodeVfx, p, Quaternion.identity);
            SetLayerRecursively(go, LayerMask.NameToLayer("Default"));
            
            var systems = go.GetComponentsInChildren<ParticleSystem>(true);
            float longest = 0f;

            foreach (var ps in systems)
            {
                var main = ps.main;
                main.cullingMode     = ParticleSystemCullingMode.AlwaysSimulate;
                main.startDelay      = 0f;
                main.simulationSpace = ParticleSystemSimulationSpace.World;
                main.playOnAwake     = true;

                var em = ps.emission; em.enabled = true;

                ps.Clear(true);
                ps.Play(true);

                longest = Mathf.Max(longest, main.duration + GetStartLifetimeMax(main));
            }

#if UNITY_VISUAL_EFFECT_GRAPH
            foreach (var vfx in go.GetComponentsInChildren<VisualEffect>(true))
            {
                vfx.playRate = 1f;
                vfx.Play();
            }
#endif
            StartCoroutine(VerifyAndKickParticles(go));
            float life = Mathf.Max(vfxLifetime, longest + 0.25f);
            Destroy(go, life);
        }

        // empuje + daño
        var playerRb = target ? target.GetComponent<Rigidbody>() : null;
        if (playerRb)
            playerRb.AddExplosionForce(explosionForce, p, explosionRadius, upwardsModifier, ForceMode.Impulse);

        if (gc && !EscudoJugador.EscudoActivoGlobal)
        {
            float before = gc.Combustible;
            gc.Combustible = Mathf.Max(0f, before - fuelDrain);
            if (gc.Combustible <= 0f) { gc.Quitar_Vida(1); gc.ResetCombustible(); }
        }

        if (explodeSfx) AudioSource.PlayClipAtPoint(explodeSfx, p, sfxVolume);

        // Lanza la ráfaga de proyectiles hacia el Player
        if (useMissileVariation && projectilePrefab && projectileCount > 0)
            LaunchVolleyImmediate();
        
        Destroy(gameObject);
    }
    
    void LaunchVolleyImmediate()
    {
        // Dirección base mirando al player en el plano XZ
        Vector3 baseDir = target ? (target.position - transform.position) : transform.forward;
        baseDir.y = 0f; 
        if (baseDir.sqrMagnitude < 0.001f) baseDir = transform.forward;
        baseDir.Normalize();

        // Punto base elevado + un poco hacia adelante para que no se toquen
        Vector3 spawnBase = transform.position 
                            + Vector3.up * projectileSpawnYOffset
                            + baseDir     * projectileForwardOffset;

        // Vector lateral (perpendicular) para espaciar físicamente los misiles
        Vector3 right = Vector3.Cross(Vector3.up, baseDir).normalized;

        int n = Mathf.Max(1, projectileCount);
        float half = (n - 1) * 0.5f;

        for (int i = 0; i < n; ++i)
        {
            // índice centrado (-half ... +half)
            float t = (n == 1) ? 0f : (i - half);

            // 1) Offset lateral en el espacio → MISILES NACES SEPARADOS
            Vector3 spawnPos = spawnBase + right * (t * projectileLateralSpacing);

            // 2) Offset angular para abrir el abanico
            float angle = (n == 1) ? 0f : (t / half) * (projectileCone * 0.5f);
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up) * Quaternion.LookRotation(baseDir, Vector3.up);

            GameObject go = Instantiate(projectilePrefab, spawnPos, rot);

            var hp = go.GetComponent<HomingProjectile>();
            if (hp) hp.targetTag = targetTag;
        }
    }
    
    IEnumerator LaunchVolley()
    {
        Vector3 baseDir = target ? (target.position - transform.position) : transform.forward;
        baseDir.y = 0f;
        if (baseDir.sqrMagnitude < 0.001f) baseDir = transform.forward;
        baseDir.Normalize();

        Vector3 spawnBase = transform.position + Vector3.up * projectileSpawnYOffset;

        int n = Mathf.Max(1, projectileCount);
        float half = (n - 1) * 0.5f;

        for (int i = 0; i < n; ++i)
        {
            // Distribuye bien los ángulos en abanico
            float angle = (n == 1) ? 0f : ((i - half) / half) * (projectileCone * 0.5f);
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up) * Quaternion.LookRotation(baseDir, Vector3.up);

            GameObject go = Instantiate(projectilePrefab, spawnBase, rot);
            var hp = go.GetComponent<HomingProjectile>();
            if (hp) hp.targetTag = targetTag;

            if (projectileDelayBetween > 0f)
                yield return new WaitForSeconds(projectileDelayBetween);
        }
    }

    IEnumerator VerifyAndKickParticles(GameObject go)
    {
        yield return null; yield return null;

        if (!go) yield break;

        int total = 0;
        var list = go.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in list) total += ps.particleCount;

        if (total == 0)
        {
            foreach (var ps in list)
            {
                var em = ps.emission; em.enabled = true;
                ps.Emit(30);
                ps.Play(true);
            }
        }
    }

    // ====== helpers ======
    float GetStartLifetimeMax(ParticleSystem.MainModule m)
    {
        var lt = m.startLifetime;
        switch (lt.mode)
        {
            case ParticleSystemCurveMode.TwoConstants: return lt.constantMax;
            case ParticleSystemCurveMode.TwoCurves:    return Mathf.Max(lt.curveMax.Evaluate(1f), lt.curveMin.Evaluate(1f));
            case ParticleSystemCurveMode.Curve:        return lt.curve.Evaluate(1f);
            default:                                   return lt.constant;
        }
    }
    void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform t in go.transform)
            SetLayerRecursively(t.gameObject, layer);
    }

    // ====== movimiento ======
    void MoveTick(float speedMult = 1f)
    {
        Vector3 toPlayer = (target.position - transform.position); toPlayer.y = 0f;
        Vector3 dir = toPlayer.normalized;

        // separación simple
        Collider[] near = Physics.OverlapSphere(transform.position, separationRadius);
        Vector3 sep = Vector3.zero;
        foreach (var c in near)
            if (c && c.gameObject != gameObject && c.CompareTag("Enemy"))
                sep += (transform.position - c.transform.position).normalized;

        Vector3 final = (dir + sep * separationForce).normalized;

        Quaternion look = Quaternion.LookRotation(final, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, turnSpeed * Time.fixedDeltaTime);

        rb.MovePosition(rb.position + final * (moveSpeed * speedMult) * Time.fixedDeltaTime);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.4f, 0, 0.25f);
        Gizmos.DrawWireSphere(transform.position, armDistance);
        Gizmos.color = new Color(1, 0, 0, 0.45f);
        Gizmos.DrawWireSphere(transform.position, explodeDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
#endif
}
