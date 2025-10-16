using Simplon;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class barril : MonoBehaviour
{
    [Header("Daño por toque")]
    public float _fuelDrainPerTouch = 5f;

    [Header("Efectos")]
    [SerializeField] GameObject _explosionPrefab;   // Prefab de partículas de explosión
    [SerializeField] AudioClip _explosionSound;     // Sonido de explosión
    [SerializeField] float _volumenExplosion = 1f;  // Volumen del sonido

    [Header("Conig. Explosion")]
    [Tooltip("Fuerza base de la explosión")]
    [SerializeField] float _explosionForce = 3000f;
    [Tooltip("Radio de alcance del empuje explosivo")]
    [SerializeField] float _explosionRadius = 5f;
    [Tooltip("Factor vertical de empuje (0 = plano, 1 = empuja hacia arriba)")]
    [SerializeField] float _upwardsModifier = 0.5f;

    GameControler _controller;

    void Start()
    {
        _controller = GameControler.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponentInParent<TopDownCarController>();
            Explode();
            Debug.Log("barril: colision con el player");

            // Bloqueo de daño en escudo por enemigo
            var shield = player.GetComponentInChildren<EscudoJugador>();
            if (shield != null && shield.EstaActivo())
            {
                Debug.Log("[ESCUDO] Daño bloqueado por escudo activo");
                //return;
            }
            else
            {
                float before = _controller.Combustible;

                // Restar en el GameController (fuente única)
                _controller.Combustible = Mathf.Max(0f, _controller.Combustible - _fuelDrainPerTouch);
                //_nextTime = Time.time + _cooldown;

                Debug.Log($"[ENEMIGO] Toque! Combustible GC: {before} -> {_controller.Combustible}");
            }

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

            Destroy(gameObject);
        }
    }
    void Explode()
    {
        // Instanciar explosión
        if (_explosionPrefab != null)
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

        // Reproducir sonido
        if (_explosionSound != null)
            AudioSource.PlayClipAtPoint(_explosionSound, transform.position, _volumenExplosion);
        //Destroy(gameObject);
    }
}
