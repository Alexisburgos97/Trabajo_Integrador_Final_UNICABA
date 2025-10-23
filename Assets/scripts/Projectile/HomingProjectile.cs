using UnityEngine;
using Simplon;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class HomingProjectile : MonoBehaviour
{
    [Header("Targeting")]
    public string targetTag = "Player";
    public float speed = 25f;
    public float acceleration = 0f;
    public float turnRate = 360f;          // grados/seg para girar hacia el objetivo
    public float seekHeightOffset = 0.6f;  // apunta al “centro” del auto

    [Header("Vida / Colisión")]
    public float lifeTime = 5f;
    public LayerMask hitMask = ~0;
    public GameObject hitVfx;
    public AudioClip hitSfx;
    [Range(0,1)] public float sfxVolume = 0.9f;

    [Header("Daño al jugador")]
    public float fuelDrain = 10f;
    public float hitForce = 350f;
    public float hitRadius = 2.5f;
    public float upwardsModifier = 0.25f;

    Rigidbody rb;
    Transform target;
    float life;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        GetComponent<Collider>().isTrigger = true;
    }

    void Start()
    {
        var t = GameObject.FindGameObjectWithTag(targetTag);
        if (t) target = t.transform;
        life = lifeTime;
    }

    void FixedUpdate()
    {
        if (!target)
        {
            rb.linearVelocity = transform.forward * speed;
            TickLife();
            return;
        }

        // dirección hacia un punto al centro/altura del auto
        Vector3 aim = (target.position + Vector3.up * seekHeightOffset) - transform.position;
        aim.y = 0f;

        if (aim.sqrMagnitude > 0.001f)
        {
            var desired = Quaternion.LookRotation(aim.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desired, turnRate * Time.fixedDeltaTime);
        }

        // acelerar + avanzar
        speed += acceleration * Time.fixedDeltaTime;
        rb.linearVelocity = transform.forward * speed;

        TickLife();
    }

    void TickLife()
    {
        life -= Time.fixedDeltaTime;
        if (life <= 0f) Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        // Ignorar otros enemigos / proyectiles si querés
        if (other.CompareTag("Enemy")) return;

        // ¿Golpeó al Player?
        var player = other.GetComponentInParent<TopDownCarController>();
        if (player)
        {
            // Empuje estilo explosión (suave y local)
            var prb = player.GetComponent<Rigidbody>();
            if (prb)
                prb.AddExplosionForce(hitForce, transform.position, hitRadius, upwardsModifier, ForceMode.Impulse);

            // Daño a combustible (respetando escudo)
            var gc = GameControler.Instance;
            if (gc && !EscudoJugador.EscudoActivoGlobal)
            {
                float before = gc.Combustible;
                gc.Combustible = Mathf.Max(0f, before - fuelDrain);
                if (gc.Combustible <= 0f) { gc.Quitar_Vida(1); gc.ResetCombustible(); }
            }

            // FX/SFX
            if (hitVfx) Destroy(Instantiate(hitVfx, transform.position, Quaternion.identity), 1.5f);
            if (hitSfx) AudioSource.PlayClipAtPoint(hitSfx, transform.position, sfxVolume);

            Destroy(gameObject);
            return;
        }

        // Cualquier otro impacto sólido del escenario
        if (((1 << other.gameObject.layer) & hitMask) != 0)
        {
            if (hitVfx) Destroy(Instantiate(hitVfx, transform.position, Quaternion.identity), 1.5f);
            if (hitSfx) AudioSource.PlayClipAtPoint(hitSfx, transform.position, sfxVolume);
            Destroy(gameObject);
        }
    }
}
