using UnityEngine;

public class Cilindro_main : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 5f;          // Velocidad de desplazamiento
    [SerializeField] float _maxDistance = 20f;       // Distancia máxima antes de autodestruirse

    [SerializeField] float _fuerzaMinPincho = 5f;   // Fuerza mínima
    [SerializeField] float _fuerzaMaxPincho = 15f;  // Fuerza máxima
    [SerializeField] Transform[] _pinchos;           // Lista de pinchos hijos

    [Header("Efectos")]
    [SerializeField] GameObject _explosionPrefab;   // Prefab de partículas de explosión
    [SerializeField] AudioClip _explosionSound;     // Sonido de explosión
    [SerializeField] float _volumenExplosion = 1f;  // Volumen del sonido
    [SerializeField] Vector3 _startPos;

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
            //Destroy(gameObject);
            Explode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Al chocar con el jugador dispara pinchos y destruye
            DispararPinchos();
            //Destroy(gameObject);
            Explode();
            Debug.Log("Cilindro: colision con el player");
        }
    }

    void DispararPinchos()
    {
        foreach (Transform pincho in _pinchos)
        {
            pincho.SetParent(null); // Lo sacamos del cilindro

            Rigidbody rb = pincho.gameObject.GetComponent<Rigidbody>();
            if (rb == null)
                rb = pincho.gameObject.AddComponent<Rigidbody>();

            // Fuerza aleatoria entre min y max
            float fuerzaAleatoria = Random.Range(_fuerzaMinPincho, _fuerzaMaxPincho);

            // Disparo en el eje forward del pincho
            rb.AddForce(pincho.forward * fuerzaAleatoria, ForceMode.Impulse);

            Pincho script = pincho.GetComponent<Pincho>();
            if (script == null)
                script = pincho.gameObject.AddComponent<Pincho>();

            script.ActivarPincho(); // 🔥 activamos el pincho recién aquí
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

        Destroy(gameObject);
    }
}
