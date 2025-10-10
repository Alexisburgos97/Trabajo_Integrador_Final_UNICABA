using UnityEngine;

public class Pincho : MonoBehaviour
{
    [SerializeField] float _lifeTime = 3f;
    [SerializeField] float _maxDistance = 15f;

    [Header("Efectos")]
    [SerializeField] GameObject _explosionPrefab;   // Prefab de partículas
    [SerializeField] AudioClip _explosionSound;     // Sonido de explosión
    [SerializeField] float _volumenExplosion = 0.7f; // Volumen (un poco más bajo que el cilindro)

    [Header("Conig. Explosion")]
    [Tooltip("Fuerza base de la explosión")]
    [SerializeField] float _explosionForce = 3000f;
    [Tooltip("Radio de alcance del empuje explosivo")]
    [SerializeField] float _explosionRadius = 5f;
    [Tooltip("Factor vertical de empuje (0 = plano, 1 = empuja hacia arriba)")]
    [SerializeField] float _upwardsModifier = 0.5f;
    
    private Vector3 _startPos;
    private bool _activo = false; // control de activación
    void Start()
    {
        _startPos = transform.position;
        //Destroy(gameObject, _lifeTime); // autodestrucción por tiempo
    }

    void Update()
    {
        if (Vector3.Distance(_startPos, transform.position) >= _maxDistance)
        {
            Explode();
            Destroy(gameObject); // autodestrucción por distancia
        }
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject); // al chocar desaparece
        Debug.Log("Pinchos: otra colision");
    }
    */
    private void OnTriggerEnter(Collider other)
    {
        if (!_activo) return; // ignorar colisiones mientras está en el cilindro

        if (other.CompareTag("Player"))
        {
            Explode();
            Debug.Log("Pinchos: colision con el player");

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

            Destroy(gameObject); // ahora sí empieza el conteo de vida
            
        }
        else
        {
            Explode();
            Debug.Log("Pinchos: otra colision");
            Destroy(gameObject); // ahora sí empieza el conteo de vida
            
        }
    }

    void Explode()
    {
        if (_explosionPrefab != null)
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

        if (_explosionSound != null)
            AudioSource.PlayClipAtPoint(_explosionSound, transform.position, _volumenExplosion);

        Destroy(gameObject);
    }
    
    public void ActivarPincho()
    {
        _activo = true;
        _startPos = transform.position; // reiniciamos punto de partida
        //Destroy(gameObject, _lifeTime); // ahora sí empieza el conteo de vida
    }
}


