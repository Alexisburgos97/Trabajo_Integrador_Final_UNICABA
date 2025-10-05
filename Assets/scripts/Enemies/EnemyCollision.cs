using UnityEngine;
using Simplon; // para GameControler

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class EnemyTouchDamage : MonoBehaviour
{
    [Header("Daño por toque")]
    public float fuelDrainPerTouch = 5f;   // cuánto baja por TOQUE
    public float cooldown = 0.3f;          // cada cuánto vuelve a aplicar si sigue tocando

    [Header("Feedback (opcionales)")]
    public GameObject hitVfxPrefab;
    public AudioClip hitSfx;
    public float sfxVolume = 0.9f;
    public float knockbackForce = 6f;      // empujón suave

    float _nextTime;

    void OnCollisionEnter(Collision c)  { Apply(c.collider, c, true); }
    void OnCollisionStay (Collision c)  { Apply(c.collider, c, false); }

    void Apply(Component other, Collision col, bool isFirstTouch)
    {
        // ¿es el player?
        var player = other.GetComponentInParent<TopDownCarController>();
        if (player == null) return;
        
        // Obtener PlayerStats del jugador
        var stats = other.GetComponentInParent<PlayerStats>();
        if (stats == null) return;

        // DAÑO INMEDIATO en el primer toque
        if (isFirstTouch || Time.time >= _nextTime)
        {
            // Bloqueo de daño en escudo por enemigo
            var shield = player.GetComponentInChildren<EscudoJugador>();
            if (shield != null && shield.EstaActivo())
            {
                Debug.Log("[ESCUDO] Daño bloqueado por escudo activo");
                //return;
            }
            else
            {
                float before = stats.Fuel;
                stats.SpendFuel(fuelDrainPerTouch);
                Debug.Log($"[ENEMIGO] Toque! Combustible: {before} -> {stats.Fuel}");
            }
            
            _nextTime = Time.time + cooldown;

            // Knockback suave para que no se “pegue”
            var rbPlayer = other.GetComponentInParent<Rigidbody>();
            if (rbPlayer != null)
            {
                Vector3 dir = (rbPlayer.worldCenterOfMass - transform.position).normalized;
                dir.y = 0f;
                rbPlayer.AddForce(dir * knockbackForce, ForceMode.Impulse);
            }

            // VFX/SFX
            if (hitVfxPrefab)
            {
                Vector3 p = col != null && col.contactCount > 0 ? col.GetContact(0).point
                                                               : other.transform.position;
                var vfx = Instantiate(hitVfxPrefab, p, Quaternion.identity);
                Destroy(vfx, 0.25f);
            }
            if (hitSfx)
            {
                Vector3 p = col != null && col.contactCount > 0 ? col.GetContact(0).point
                    : other.transform.position;

                // Crear un objeto temporal con AudioSource
                GameObject temp = new GameObject("TempAudio");
                temp.transform.position = p;

                var a = temp.AddComponent<AudioSource>();
                a.clip = hitSfx;
                a.volume = sfxVolume;

                a.time = 3.9f;  

                a.Play();

                // destruir después de reproducir lo que queda del clip
                Destroy(temp, a.clip.length - a.time);
            }
        }
    }
}
