using UnityEngine;
using Simplon;

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
        // ¿es el player?
        var player = other.GetComponentInParent<TopDownCarController>();
        if (player == null) return;

        if (isFirstTouch )
        {
            float before = _controller.Combustible;

            // ✅ Restar en el GameController (fuente única)
            _controller.Combustible = Mathf.Max(0f, _controller.Combustible - _fuelDrainPerTouch);
            //_nextTime = Time.time + _cooldown;

            Debug.Log($"[ENEMIGO] Toque! Combustible GC: {before} -> {_controller.Combustible}");

            if (_controller.Combustible <= 0f)
            {
                _controller.Quitar_Vida(1);
                _controller.ResetCombustible();
            }

            // Knockback tipo explosión
            var rbPlayer = other.GetComponentInParent<Rigidbody>();
            if (rbPlayer != null)
            {
                Vector3 explosionPos = transform.position;

                // Aplica fuerza radial
                rbPlayer.AddExplosionForce(
                    _explosionForce,        // fuerza total
                    explosionPos,           // origen
                    _explosionRadius,       // radio
                    _upwardsModifier,       // empuje vertical
                    ForceMode.Impulse       // tipo de fuerza
                );

                // Debug opcional (ver en escena)
                Debug.DrawLine(explosionPos, rbPlayer.worldCenterOfMass, Color.red, 1f);
            }


            // VFX/SFX (igual que ya tenías)
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
            Destroy(gameObject);
        }
    }
}
