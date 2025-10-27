using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Cohete_copy : MonoBehaviour
{
    [Header("Movimiento")]
    public float initialSpeed = 20f;        // velocidad del impulso inicial
    public float thrust = 30f;              // aceleración mientras homing
    public float maxSpeed = 40f;
    public float turnSpeed = 180f;          // grados por segundo
    public float homingDelay = 0.25f;       // tiempo antes de empezar a seguir
    public float lifeTime = 8f;

    [Header("VFX / SFX")]
    public GameObject hitVfxPrefab;
    public AudioClip hitSfx;
    public float sfxVolume = 1f;

    Rigidbody _rb;
    Transform target;
    bool isLaunched = false;
    float spawnTime;
    bool homingActive = false;

    public Transform Target => target;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Launch(Transform assignedTarget, Vector3 initialDirection)
    {
        target = assignedTarget;
        isLaunched = true;
        homingActive = false;
        spawnTime = Time.time;
        _rb.linearVelocity = initialDirection.normalized * initialSpeed;
        // opcional: orientar el cohete hacia adelante
        transform.rotation = Quaternion.LookRotation(initialDirection.normalized, Vector3.up);
    }

    void FixedUpdate()
    {
        if (!isLaunched) return;

        // tiempo de vida
        if (Time.time - spawnTime > lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        // activar homing después de delay
        if (!homingActive && Time.time - spawnTime >= homingDelay)
            homingActive = true;

        if (homingActive && target != null)
        {
            Vector3 toTarget = (target.position - transform.position).normalized;
            // rotación suave
            Quaternion targetRot = Quaternion.LookRotation(toTarget, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.fixedDeltaTime);

            // aplicar fuerza hacia adelante (acelerar)
            _rb.AddForce(transform.forward * thrust * Time.fixedDeltaTime, ForceMode.VelocityChange);

            // limitar velocidad
            if (_rb.linearVelocity.magnitude > maxSpeed)
                _rb.linearVelocity = _rb.linearVelocity.normalized * maxSpeed;
        }
        else
        {
            // si no hay target o todavía no homing, mantener velocidad (inercia)
            // opcional: pequeña deceleración o drag automático
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // manejar colisión con enemigo
        var other = collision.collider;
        if (other.CompareTag("Enemy"))
        {
            var enemyScript = other.GetComponent<EnemyTouchDamage>();
            Vector3 contactPoint = (collision.contactCount > 0) ? collision.GetContact(0).point : other.transform.position;

            if (enemyScript != null)
            {
                // Llamamos al método que vamos a agregar en el script del enemigo
                enemyScript.ExplodeFromProjectile(contactPoint);
            }
            else
            {
                // si no tiene el script, simplemente destruir
                Destroy(other.gameObject);
            }

            // efectos del impacto
            if (hitVfxPrefab)
            {
                var vfx = Instantiate(hitVfxPrefab, contactPoint, Quaternion.identity);
                Destroy(vfx, 2f);
            }

            if (hitSfx)
            {
                GameObject tmp = new GameObject("SfxTemp");
                tmp.transform.position = contactPoint;
                var a = tmp.AddComponent<AudioSource>();
                a.clip = hitSfx;
                a.volume = sfxVolume;
                a.Play();
                Destroy(tmp, hitSfx.length + 0.1f);
            }

            // destruir cohete
            Destroy(gameObject);
        }
        else
        {
            // si choca con cualquier otra cosa también puede explotar (opcional)
            // Destroy(gameObject);
        }
    }
}
