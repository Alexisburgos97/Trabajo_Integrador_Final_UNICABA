using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Enemy_v2 : MonoBehaviour
{
    [Header("Configuración del objetivo")]
    public string _targetTag = "Player";
    private Transform _target;

    [Header("Movimiento")]
    [SerializeField] float _moveSpeed = 8f;
    [SerializeField] float _turnSpeed = 180f;

    [Header("Evitar superposición")]
    [SerializeField] float _separationRadius = 2f;
    [SerializeField] float _separationForce = 5f;

    [Header("Dash / Teletransporte")]
    [SerializeField] float _dashDistance = 15f;
    [SerializeField] float _dashSpeed = 40f;
    [SerializeField] float _dashCooldown = 5f;
    [SerializeField] float _teleportDistance = 40f;
    [SerializeField] float _teleportRadiusAroundPlayer = 10f;
    [SerializeField] float _teleportCooldown = 8f;

    [Header("Efectos")]
    [SerializeField] GameObject _portalPrefab; // Prefab del portal (VFX Graph)
    [SerializeField] ParticleSystem _dashTrail; // Trail del dash (Particle System)
    [SerializeField] float _portalLifetime = 1.3f; // Duración del portal (igual al Tiempo_efecto del VFX)

    private Rigidbody _rb;
    private bool _canDash = true;
    private bool _canTeleport = true;

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
        if (targetObj) _target = targetObj.transform;
    }

    void FixedUpdate()
    {
        if (!_target) return;

        float distToPlayer = Vector3.Distance(transform.position, _target.position);

        // --- Teleport si está muy lejos ---
        if (_canTeleport && distToPlayer > _teleportDistance)
        {
            StartCoroutine(TeleportNearPlayer());
            return;
        }

        // --- Dash si está a media distancia ---
        if (_canDash && distToPlayer > _dashDistance)
        {
            StartCoroutine(DashToPlayer());
            return;
        }

        // Movimiento normal si nada especial ocurre
        MoveTowardsPlayer();
    }

    // --- Movimiento normal ---
    void MoveTowardsPlayer()
    {
        Vector3 dir = (_target.position - transform.position).normalized;
        dir.y = 0f;

        // Separación de otros enemigos
        Collider[] near = Physics.OverlapSphere(transform.position, _separationRadius);
        Vector3 sep = Vector3.zero;
        foreach (var col in near)
        {
            if (col.gameObject != gameObject && col.CompareTag("Enemy"))
                sep += (transform.position - col.transform.position).normalized;
        }

        Vector3 finalDir = (dir + sep * _separationForce).normalized;
        Quaternion look = Quaternion.LookRotation(finalDir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, _turnSpeed * Time.fixedDeltaTime);
        _rb.MovePosition(_rb.position + finalDir * _moveSpeed * Time.fixedDeltaTime);
    }

    // --- Dash ---
    IEnumerator DashToPlayer()
    {
        _canDash = false;

        if (_dashTrail != null)
            _dashTrail.Play();

        Vector3 dashDir = (_target.position - transform.position).normalized;
        float dashTime = _dashDistance / _dashSpeed;

        float timer = 0f;
        while (timer < dashTime)
        {
            _rb.MovePosition(_rb.position + dashDir * _dashSpeed * Time.fixedDeltaTime);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (_dashTrail != null)
            _dashTrail.Stop();

        yield return new WaitForSeconds(_dashCooldown);
        _canDash = true;
    }

    // --- Teletransporte con efecto ---
    IEnumerator TeleportNearPlayer()
    {
        _canTeleport = false;

        // Elegir posición aleatoria cerca del jugador
        Vector3 randomPos = _target.position + (Random.insideUnitSphere * _teleportRadiusAroundPlayer);
        randomPos.y = transform.position.y;

        // Crear el portal en el punto donde aparecerá el enemigo
        GameObject portal = Instantiate(_portalPrefab, randomPos, Quaternion.identity);

        // Esperar que termine la animación del portal antes de mover al enemigo
        yield return new WaitForSeconds(_portalLifetime);

        transform.position = randomPos;

        // Destruir el portal después del efecto
        Destroy(portal, _portalLifetime);

        yield return new WaitForSeconds(_teleportCooldown);
        _canTeleport = true;
    }
}
