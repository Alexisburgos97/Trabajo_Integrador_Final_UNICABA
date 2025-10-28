using System;
using System.Collections;
using System.Data.Common;
using UnityEngine;

    
public class Missile_v2 : MonoBehaviour {
    [Header("REFERENCES")] 
    [SerializeField] private Enemy_v2 _target;
    [SerializeField] private GameObject _explosionPrefab;

    [Header("Movimiento")] 
    [SerializeField] private float _speed = 15;
    [SerializeField] private float _rotateSpeed = 95;
    [SerializeField] float _initialSpeed = 20f;        // velocidad del impulso inicial
    [SerializeField] float lifeTime = 8f;


    [Header("PREDICTION")] 
    [SerializeField] private float _maxDistancePredict = 100;
    [SerializeField] private float _minDistancePredict = 5;
    [SerializeField] private float _maxTimePrediction = 5;
    private Vector3 _standardPrediction, _deviatedPrediction;

    [Header("DEVIATION")] 
    [SerializeField] private float _deviationAmount = 50;
    [SerializeField] private float _deviationSpeed = 2;

    [Header("config lanzamiento")]
    [SerializeField] float _tiempo_lanzamiento = 0.25f;
    Vector3 _Dir_incial;

    Rigidbody _rb;
    bool _lanzamiento = true;
    bool _activ_corutina = false;
    void Start()
    {
        //obtener el rigidbody del gameobject que contiene el script
        _rb = gameObject.GetComponent<Rigidbody>();
    }

    public void set_target(Enemy_v2 enemy, Vector3 dir, float tiempoLanzamiento=0)
    {
        _target = enemy;//referencia del enemigo
        _Dir_incial = dir; //direccion del lanzamiento inicial
        if (tiempoLanzamiento > 0)
        {
            _tiempo_lanzamiento = tiempoLanzamiento; // se puede reescribir el tiempo de lanzamiento    
        }
           
    }

    private void FixedUpdate() {
        if (_lanzamiento)
        {
            //activar el retardo para el lanzamiento una sola vez
            if (!_activ_corutina)
            {
                StartCoroutine(ContarTiempo());
                _activ_corutina = true;
            }

            _rb.linearVelocity = _Dir_incial.normalized * _initialSpeed;
            // opcional: orientar el cohete hacia adelante
            transform.rotation = Quaternion.LookRotation(_Dir_incial.normalized, Vector3.up);
        }
        else
        {
            _rb.linearVelocity = transform.forward * _speed;
            if (_target != null)
            {
                var leadTimePercentage = Mathf.InverseLerp(_minDistancePredict, _maxDistancePredict, Vector3.Distance(transform.position, _target.transform.position));

                PredictMovement(leadTimePercentage);

                AddDeviation(leadTimePercentage);

                RotateRocket();
            }
            else
            {
                destruir();
            }
            
        }
        
    }

    private void PredictMovement(float leadTimePercentage) {
        var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);

        _standardPrediction = _target._Rb.position + _target._Rb.linearVelocity  * predictionTime;
    }

    private void AddDeviation(float leadTimePercentage) {
        var deviation = new Vector3(Mathf.Cos(Time.time * _deviationSpeed), 0, 0);
        
        var predictionOffset = transform.TransformDirection(deviation) * _deviationAmount * leadTimePercentage;

        _deviatedPrediction = _standardPrediction + predictionOffset;
    }

    private void RotateRocket()
    {
        var heading = _deviatedPrediction - transform.position;

        var rotation = Quaternion.LookRotation(heading);
        _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, _rotateSpeed * Time.deltaTime));
    }

    /*
        private void OnCollisionEnter(Collision collision)
        {
            Collider other = collision.collider;

            if (other.CompareTag("Player")) return;


            if (_explosionPrefab) Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

            EnemyTouchDamage enemyScript = other.GetComponent<EnemyTouchDamage>();
            Vector3 contactPoint = (collision.contactCount > 0) ? collision.GetContact(0).point : other.transform.position;
            if (enemyScript != null)
            {
                // Llamamos al método que vamos a agregar en el script del enemigo
                enemyScript.ExplodeFromProjectile(contactPoint);
            }
            Destroy(gameObject);
        }
    */
    void OnTriggerEnter(Collider other)
    {

        EnemyTouchDamage enemyScript = other.GetComponent<EnemyTouchDamage>();

        //Vector3 contactPoint = other.ClosestPoint(transform.position);
        Vector3 contactPoint = transform.position;
        if (enemyScript != null)
        {
            // Llamamos al método que vamos a agregar en el script del enemigo
            enemyScript.ExplodeFromProjectile(contactPoint);
        }
        destruir();
        
    }

    void destruir()
    {
        if (_explosionPrefab) Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        StartCoroutine(DestruirConDelay());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _standardPrediction);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_standardPrediction, _deviatedPrediction);
    }
    /*
    public void Launch(Transform assignedTarget, Vector3 initialDirection)
    {
        target = assignedTarget;
        isLaunched = true;
        homingActive = false;
        spawnTime = Time.time;
        _rb.linearVelocity = initialDirection.normalized * _initialSpeed;
        // opcional: orientar el cohete hacia adelante
        transform.rotation = Quaternion.LookRotation(initialDirection.normalized, Vector3.up);
    }
    */
    IEnumerator ContarTiempo()
    {
        // Espera la cantidad de segundos indicada
        yield return new WaitForSeconds(_tiempo_lanzamiento);

        // se detiene la fase de lanzamiento
        _lanzamiento = false;

        //Debug.Log("⏰ Tiempo terminado. Variable 'activo' = " + activo);
    }

    IEnumerator DestruirConDelay()
    {
        yield return new WaitForFixedUpdate(); // espera al final del frame de física
        Destroy(gameObject);
    }
}
