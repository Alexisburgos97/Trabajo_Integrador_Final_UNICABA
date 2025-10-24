using UnityEngine;
using Simplon;

public class Cilindro_main : MonoBehaviour
{
    [Header("Configuracion del cilindro")]
    [SerializeField] float _moveSpeed = 5f;          // Velocidad de desplazamiento
    [SerializeField] float _maxDistance = 20f;       // Distancia máxima antes de autodestruirse

    [Header("Daño por toque")]
    public float _fuelDrainPerTouch = 5f;

    [Header("Config. de los pinchos")]
    [SerializeField] float _fuerzaMinPincho = 5f;   // Fuerza mínima
    [SerializeField] float _fuerzaMaxPincho = 15f;  // Fuerza máxima
    [SerializeField] Transform[] _pinchos;           // Lista de pinchos hijos

    [Header("Efectos")]
    [SerializeField] GameObject _explosionPrefab;   // Prefab de partículas de explosión
    [SerializeField] AudioClip _explosionSound;     // Sonido de explosión
    [SerializeField] float _volumenExplosion = 1f;  // Volumen del sonido
    [SerializeField] Vector3 _startPos;

    [Header("Conig. Explosion")]
    [Tooltip("Fuerza base de la explosión")]
    [SerializeField] float _explosionForce = 3000f;
    [Tooltip("Radio de alcance del empuje explosivo")]
    [SerializeField] float _explosionRadius = 5f;
    [Tooltip("Factor vertical de empuje (0 = plano, 1 = empuja hacia arriba)")]
    [SerializeField] float _upwardsModifier = 0.5f;

    GameControler _controler;
    void Start()
    {
        _startPos = transform.position;
    }

    void Update()
    {
        // Mover en eje X global, ignorando la rotación de la animación
        transform.Translate(Vector3.left * _moveSpeed * Time.deltaTime, Space.World);

        // Si recorrió más que la distancia máxima → dispara pinchos y destruye
        if (Vector3.Distance(_startPos, transform.position) >= _maxDistance)
        {
            DispararPinchos();
            Explode();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Al chocar con el jugador dispara pinchos y destruye
            Explode();
            DispararPinchos();
            Debug.Log("Cilindro: colision con el player");

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
                // ⚠️ Solo se bloquea el daño, no el resto
                //if (!EscudoJugador.EscudoActivoGlobal)
               // {
                    // ✅ Solo aplica daño si el escudo NO está activo
                    _controler.Combustible = Mathf.Max(0f, _controler.Combustible - _fuelDrainPerTouch);
                
                    if (_controler.Combustible <= 0f)
                    {
                        _controler.Quitar_Vida(1);
                        _controler.ResetCombustible();
                    }
                //}
            }

            Destroy(gameObject);
        }
    }

    // void DispararPinchos()
    // {
    //     foreach (Transform pincho in _pinchos)
    //     {
    //         pincho.SetParent(null); // Lo sacamos del cilindro
    //
    //         Rigidbody rb = pincho.gameObject.GetComponent<Rigidbody>();
    //         if (rb == null)
    //             rb = pincho.gameObject.AddComponent<Rigidbody>();
    //
    //         // Fuerza aleatoria entre min y max
    //         float fuerzaAleatoria = Random.Range(_fuerzaMinPincho, _fuerzaMaxPincho);
    //
    //         // Disparo en el eje forward del pincho
    //         rb.AddForce(pincho.forward * fuerzaAleatoria, ForceMode.Impulse);
    //
    //         Pincho script = pincho.GetComponent<Pincho>();
    //         if (script == null)
    //             script = pincho.gameObject.AddComponent<Pincho>();
    //
    //         script.ActivarPincho(); // 🔥 activamos el pincho recién aquí
    //     }
    //     Destroy(gameObject);
    // }
    
    void DispararPinchos()
    {
        if (_pinchos == null) return;

        foreach (var pincho in _pinchos)
        {
            if (pincho == null) continue;                   // ⛔ ya destruido o no asignado
            if (pincho.parent == null) continue;            // ya despadreado en otra llamada

            pincho.SetParent(null, true);                   // ✅ conservar world transform

            // rigidbody
            if (!pincho.TryGetComponent<Rigidbody>(out var rb))
                rb = pincho.gameObject.AddComponent<Rigidbody>();

            float fuerza = Random.Range(_fuerzaMinPincho, _fuerzaMaxPincho);
            rb.AddForce(pincho.forward * fuerza, ForceMode.Impulse);

            // script del pincho
            if (!pincho.TryGetComponent<Pincho>(out var script))
                script = pincho.gameObject.AddComponent<Pincho>();

            script.ActivarPincho();
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
