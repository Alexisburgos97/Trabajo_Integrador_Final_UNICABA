using UnityEngine;

public class Cilindro_main : MonoBehaviour
{
    [Header("Configuracion del cilindro")]
    [SerializeField] float _moveSpeed = 5f;          // Velocidad de desplazamiento
    [SerializeField] float _maxDistance = 20f;       // Distancia máxima antes de autodestruirse

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
    [SerializeField] float _knockbackForce = 6f;
    [SerializeField] float _explosionRadius = 5f;      // radio de la onda expansiva
    [SerializeField] float _explosionPower = 50f; // potencia de la explosión

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
                
                _explosionPower = _knockbackForce * 50f; // potencia de la explosión
                rbPlayer.AddExplosionForce(_explosionPower, explosionPos, _explosionRadius, 0f, ForceMode.Impulse);
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
