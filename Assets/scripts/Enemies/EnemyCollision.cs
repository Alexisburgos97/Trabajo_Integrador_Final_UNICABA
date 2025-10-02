using UnityEngine;
using Simplon;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class EnemyTouchDamage : MonoBehaviour
{
    [Header("Daño por toque")]
    public float fuelDrainPerTouch = 5f;
    public float cooldown = 0.3f;

    [Header("Feedback (opcionales)")]
    public GameObject hitVfxPrefab;
    public AudioClip hitSfx;
    public float sfxVolume = 0.9f;
    public float knockbackForce = 6f;

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

        if (isFirstTouch || Time.time >= _nextTime)
        {
            float before = _controller.Combustible;

            // ✅ Restar en el GameController (fuente única)
            _controller.Combustible = Mathf.Max(0f, _controller.Combustible - fuelDrainPerTouch);
            _nextTime = Time.time + cooldown;

            Debug.Log($"[ENEMIGO] Toque! Combustible GC: {before} -> {_controller.Combustible}");

            if (_controller.Combustible <= 0f)
            {
                _controller.QuitarVida(1);
                _controller.ResetCombustible();
            }

            // Knockback suave
            var rbPlayer = other.GetComponentInParent<Rigidbody>();
            if (rbPlayer != null)
            {
                Vector3 dir = (rbPlayer.worldCenterOfMass - transform.position).normalized;
                dir.y = 0f;
                rbPlayer.AddForce(dir * knockbackForce, ForceMode.Impulse);
            }

            // VFX/SFX (igual que ya tenías)
            if (hitVfxPrefab)
            {
                Vector3 p = (col != null && col.contactCount > 0) ? col.GetContact(0).point : other.transform.position;
                var vfx = Instantiate(hitVfxPrefab, p, Quaternion.identity);
                Destroy(vfx, 0.25f);
            }
            if (hitSfx)
            {
                Vector3 p = (col != null && col.contactCount > 0) ? col.GetContact(0).point : other.transform.position;
                var temp = new GameObject("TempAudio");
                temp.transform.position = p;
                var a = temp.AddComponent<AudioSource>();
                a.clip = hitSfx;
                a.volume = sfxVolume;
                a.Play();
                Destroy(temp, a.clip.length);
            }
            Destroy(gameObject);
        }
    }
}
