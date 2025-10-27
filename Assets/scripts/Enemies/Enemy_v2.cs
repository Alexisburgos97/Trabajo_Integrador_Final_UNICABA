using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Enemy_v2 : MonoBehaviour
{
    [Header("Configuración del objetivo")]
    public string _targetTag = "Player";
    private Transform _target;
    [Header("Prediccion de movimiento")]
    [SerializeField] private float _size = 10;

    [Header("Movimiento")]
    [SerializeField] float _moveSpeed = 15f;
    [SerializeField] float _turnSpeed = 180f;

    [Header("Evitar superposición")]
    [SerializeField] float _separationRadius = 5f;
    [SerializeField] float _separationForce = 60f;

    [Header("Dash / Teletransporte")]
    [SerializeField] float _dashDistance = 15f;
    [SerializeField] float _dashSpeed = 40f;
    [SerializeField] float _dashCooldown = 5f;
    [SerializeField] float _teleportDistance = 40f;
    [SerializeField] float _teleportRadiusAroundPlayer = 10f;
    [SerializeField] float _teleportCooldown = 6f;

    [Header("Efectos")]
    [SerializeField] GameObject _portalPrefab; // Prefab del portal (VFX Graph)
    [SerializeField] ParticleSystem _dashTrail; // Trail del dash (Particle System)
    [SerializeField] float _portalLifetime = 0.7f; // Duración del portal (igual al Tiempo_efecto del VFX)

    [Header("Rush inicial")]
    [SerializeField] float _rushMultiplier = 2f;        // Velocidad multiplicada al inicio
    [SerializeField] float _rushStopDistance = 20f;     // Distancia al player para salir del modo rush
    [SerializeField] float _rushMaxTime = 6f;           // Tiempo máximo antes de forzar teleport

    private Rigidbody _rb,_esteRb;
    public Rigidbody _Rb => _esteRb;

    //public Vector3 _linear_Velocity;
        
    private bool _canDash = false;
    private bool _canTeleport = false;
    private bool _isRushing = true;
    private float _rushTimer = 0f;

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
        _esteRb = gameObject.GetComponent<Rigidbody>();
    }

    /*
    void Update()
    {
        //mantien actializada la posicion del enemigo y predice el movimiento
        Vector3 dir;
        if (_isRushing)
        {
            dir = new Vector3(Mathf.Cos(Time.time * _dashSpeed) * _size, Mathf.Sin(Time.time * _dashSpeed) * _size);
        }
        else
        {
            dir = new Vector3(Mathf.Cos(Time.time * _moveSpeed) * _size, Mathf.Sin(Time.time * _moveSpeed) * _size);
        }
        //_esteRb.linearVelocity = dir;
        _linear_Velocity = dir;

    }
    */
    void FixedUpdate()
    {
        if (!_target) return;

        float distToPlayer = Vector3.Distance(transform.position, _target.position);

        // --- Fase inicial Rush ---
        if (_isRushing)
        {
            _rushTimer += Time.fixedDeltaTime;
            MoveTowardsPlayer(_moveSpeed * _rushMultiplier);

            // Si se acerca lo suficiente o excede el tiempo máximo → salir del modo rush
            if (distToPlayer < _rushStopDistance)
            {
                EndRush();
            }
            else if (_rushTimer >= _rushMaxTime)
            {
                // Teletransportar para iniciar comportamiento normal
                StartCoroutine(EndRushWithTeleport());
            }
            return;
        }

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

        // --- Movimiento normal ---
        MoveTowardsPlayer(_moveSpeed);
    }

    // --- Terminar rush sin teleport ---
    void EndRush()
    {
        _isRushing = false;
        _canDash = true;
        _canTeleport = true;
    }

    // --- Terminar rush con teleport ---
    IEnumerator EndRushWithTeleport()
    {
        _isRushing = false;
        yield return TeleportNearPlayer(); // hace el teleport y activa cooldown
        _canDash = true;
        _canTeleport = true;
    }

    // --- Movimiento hacia el jugador ---
    void MoveTowardsPlayer(float speed)
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
        _rb.MovePosition(_rb.position + finalDir * speed * Time.fixedDeltaTime);
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

        // Reubicar al enemigo
        transform.position = randomPos;

        // --- Ajustar rotación para mirar al jugador ---
        Vector3 lookDir = (_target.position - transform.position).normalized;
        lookDir.y = 0f;
        transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);

        // Destruir el portal después del efecto
        Destroy(portal, _portalLifetime);

        yield return new WaitForSeconds(_teleportCooldown);
        _canTeleport = true;
    }
}