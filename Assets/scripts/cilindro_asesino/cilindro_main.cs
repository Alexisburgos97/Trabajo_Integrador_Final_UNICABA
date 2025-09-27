using UnityEngine;

public class Cilindro_main : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 5f;          // Velocidad de desplazamiento
    [SerializeField]float _maxDistance = 20f;       // Distancia máxima antes de autodestruirse

    [SerializeField] float _fuerzaMinPincho = 5f;   // Fuerza mínima
    [SerializeField] float _fuerzaMaxPincho = 15f;  // Fuerza máxima
    [SerializeField] Transform[] _pinchos;           // Lista de pinchos hijos

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
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Al chocar con el jugador dispara pinchos y destruye
            DispararPinchos();
            Destroy(gameObject);
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

        // Le agregamos script de autodestrucción
        if (pincho.GetComponent<Pincho>() == null)
            pincho.gameObject.AddComponent<Pincho>();
    }
}
}
