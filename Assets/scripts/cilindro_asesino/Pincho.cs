using UnityEngine;

public class Pincho : MonoBehaviour
{
    [SerializeField] float _lifeTime = 3f;
    [SerializeField] float _maxDistance = 15f;

    [Header("Efectos")]
    [SerializeField] GameObject _explosionPrefab;   // Prefab de partículas
    [SerializeField] AudioClip _explosionSound;     // Sonido de explosión
    [SerializeField] float _volumenExplosion = 0.7f; // Volumen (un poco más bajo que el cilindro)
    private Vector3 _startPos;
    private bool _activo = false; // control de activación
    void Start()
    {
        _startPos = transform.position;
        Destroy(gameObject, _lifeTime); // autodestrucción por tiempo
    }

    void Update()
    {
        if (Vector3.Distance(_startPos, transform.position) >= _maxDistance)
        {
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
            Debug.Log("Pinchos: colision con el player");
            Destroy(gameObject); // ahora sí empieza el conteo de vida
            Explode();
        }
        else
        {
            Debug.Log("Pinchos: otra colision");
            Destroy(gameObject); // ahora sí empieza el conteo de vida
            Explode();
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


