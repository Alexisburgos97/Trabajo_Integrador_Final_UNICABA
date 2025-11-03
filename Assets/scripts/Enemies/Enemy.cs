using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [Header("Configuración del objetivo")]
    public string _targetTag = "Player";  // Tag del objeto a perseguir
    private Transform _target;

    [Header("Movimiento")]
    public float _moveSpeed = 8f;
    public float _turnSpeed = 180f;

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
        // Busca al objetivo por Tag
        GameObject targetObj = GameObject.FindGameObjectWithTag(_targetTag);
        if (targetObj != null)
        {
            _target = targetObj.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con el tag: " + _targetTag);
        }
    }

    void FixedUpdate()
    {
        if (!_target) return;

        Vector3 dir = _target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        // Rotar hacia el objetivo
        Quaternion look = Quaternion.LookRotation(dir.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, _turnSpeed * Time.fixedDeltaTime);

        // Avanzar
        Vector3 step = transform.forward * _moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + step);
    }
}
