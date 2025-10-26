using UnityEngine;
using Simplon;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class EnemyTouchDamage : MonoBehaviour
{
    [Header("Daño por toque")]
    public float _fuelDrainPerTouch = 5f;
    public float _cooldown = 0.3f;

    [Header("Feedback (opcionales)")]
    public GameObject _hitVfxPrefab;
    public AudioClip hitSfx;
    public float _sfxVolume = 0.9f;
    
    [Header("Conig. Explosion")]
    [Tooltip("Fuerza base de la explosión")]
    [SerializeField] float _explosionForce = 300f;
    [Tooltip("Radio de alcance del empuje explosivo")]
    [SerializeField] float _explosionRadius = 5f;
    [Tooltip("Factor vertical de empuje (0 = plano, 1 = empuja hacia arriba)")]
    [SerializeField]float _upwardsModifier = 0.5f;
    float _nextTime;
    GameControler _controller;

    void Start()
    {
        _controller = GameControler.Instance;
    }

    void OnCollisionEnter(Collision c) { Apply(c.collider, c, true); }
    void OnCollisionStay(Collision c)  { Apply(c.collider, c, false); }

    void Apply(Component other, Collision col, bool isFirstTouch)
    {
        // ¿Es el player?
        var player = other.GetComponentInParent<TopDownCarController>();
        if (player == null) return;

        if (isFirstTouch)
        {
            float before = _controller.Combustible;

            // ⚠️ Solo se bloquea el daño, no el resto
            if (!EscudoJugador.EscudoActivoGlobal)
            {
                // ✅ Solo aplica daño si el escudo NO está activo
                _controller.Combustible = Mathf.Max(0f, _controller.Combustible - _fuelDrainPerTouch);
                //Debug.Log($"[ENEMIGO] Toque! Combustible GC: {before} -> {_controller.Combustible}");

                if (_controller.Combustible <= 0f)
                {
                    _controller.Quitar_Vida(1);
                    _controller.ResetCombustible();
                }
            }
            else
            {
                Debug.Log("[ENEMIGO] Escudo activo — sin daño al jugador, pero explota.");
            }

            // 💥 Knockback tipo explosión (siempre se ejecuta)
            var rbPlayer = other.GetComponentInParent<Rigidbody>();
            if (rbPlayer != null)
            {
                Vector3 explosionPos = transform.position;

                rbPlayer.AddExplosionForce(
                    _explosionForce,
                    explosionPos,
                    _explosionRadius,
                    _upwardsModifier,
                    ForceMode.Impulse
                );

                Debug.DrawLine(explosionPos, rbPlayer.worldCenterOfMass, Color.cyan, 1f);
            }

            // 💫 Efectos visuales y de sonido (también siempre se ejecutan)
            if (_hitVfxPrefab)
            {
                Vector3 p = (col != null && col.contactCount > 0) ? col.GetContact(0).point : other.transform.position;
                var vfx = Instantiate(_hitVfxPrefab, p, Quaternion.identity);
                Destroy(vfx, 0.25f);
            }

            if (hitSfx)
            {
                Vector3 p = (col != null && col.contactCount > 0) ? col.GetContact(0).point : other.transform.position;
                var temp = new GameObject("TempAudio");
                temp.transform.position = p;
                var a = temp.AddComponent<AudioSource>();
                a.clip = hitSfx;
                a.volume = _sfxVolume;
                a.Play();
                Destroy(temp, a.clip.length);
            }

            // 💀 Siempre se destruye el enemigo tras colisionar
            StartCoroutine(DestruirConDelay());
        }
    }

    // ----------------------------
    // Método público para ser llamado por proyectiles / cohetes
    // ----------------------------
    public void ExplodeFromProjectile(Vector3 contactPoint)
    {
        // knockback (si quieres, podrías calcular rb alrededor)
        if (_hitVfxPrefab)
        {
            var vfx = Instantiate(_hitVfxPrefab, contactPoint, Quaternion.identity);
            Destroy(vfx, 0.25f);
        }

        if (hitSfx)
        {
            var temp = new GameObject("TempAudio");
            temp.transform.position = contactPoint;
            var a = temp.AddComponent<AudioSource>();
            a.clip = hitSfx;
            a.volume = _sfxVolume;
            a.Play();
            Destroy(temp, a.clip.length);
        }

        // Explosión con física hacia objetos cercanos (similar a Apply)
        Vector3 explosionPos = transform.position;
        Collider[] cols = Physics.OverlapSphere(explosionPos, _explosionRadius);
        foreach (var c in cols)
        {
            var rb = c.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(_explosionForce, explosionPos, _explosionRadius, _upwardsModifier, ForceMode.Impulse);
            }
        }

        // destruir el enemigo
        StartCoroutine(DestruirConDelay());
    }
    
    IEnumerator DestruirConDelay()
{
    yield return new WaitForFixedUpdate(); // espera al final del frame de física
    Destroy(gameObject);
}
 
}
