using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [Header("Configuración del objetivo")]
    public string _targetTag = "Player";  
    private Transform _target;

    [Header("Movimiento")]
    public float _moveSpeed = 8f;
    public float _turnSpeed = 180f;

    [Header("Evitar superposición")]
    public float separationRadius = 2f;      // Distancia mínima entre enemigos
    public float separationForce = 5f;       // Intensidad de la fuerza de separación

    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void Start()
    {
        GameObject targetObj = GameObject.FindGameObjectWithTag(_targetTag);
        if (targetObj != null)
            _target = targetObj.transform;
        else
            Debug.LogWarning("No se encontró un objeto con el tag: " + _targetTag);
    }

    void FixedUpdate()
    {
        if (!_target) return;

        // --- 1️⃣ Calcular dirección hacia el jugador ---
        Vector3 dir = _target.position - transform.position;
        dir.y = 0f;

        // --- 2️⃣ Evitar superposición con otros enemigos ---
        Collider[] nearEnemies = Physics.OverlapSphere(transform.position, separationRadius);
        Vector3 separationDir = Vector3.zero;

        foreach (var col in nearEnemies)
        {
            if (col.gameObject != gameObject && col.CompareTag("Enemy"))
            {
                Vector3 away = transform.position - col.transform.position;
                float dist = away.magnitude;
                if (dist > 0)
                    separationDir += away.normalized / dist; // más fuerte cuanto más cerca
            }
        }

        // --- 3️⃣ Combinar dirección hacia el jugador + separación ---
        Vector3 finalDir = (dir.normalized + separationDir * separationForce).normalized;

        // Rotar hacia el jugador (con pequeña mezcla de dirección)
        Quaternion look = Quaternion.LookRotation(finalDir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, _turnSpeed * Time.fixedDeltaTime);

        // Avanzar
        Vector3 step = finalDir * _moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + step);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }

    void OnCollisionStay(Collision col)
{
    if (col.gameObject.CompareTag("Enemy"))
    {
        Vector3 pushDir = transform.position - col.transform.position;
        _rb.AddForce(pushDir.normalized * 20f, ForceMode.Acceleration);
    }
}

}
