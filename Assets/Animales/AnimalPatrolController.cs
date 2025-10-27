using UnityEngine;
using System.Collections;

public class AnimalPatrolController : MonoBehaviour
{
    private TopDownCarController player;
    public float moveSpeed;
    public Rigidbody theRB;
    public Animator anim;

    public float chaseRange = 15f, stopCloseRange = 4f;

    private float strafeAmount;
    public Transform[] patrolPoints;
    [HideInInspector] public int currentPatrolPoint;
    public Transform pointsHolder;
    public float pointWaitTime = 3f;
    private float waitCounter;

    public float waitToDisappear = 3f;

    public bool explode;
    private bool isDead;

    [Header("Blood Effects")]
    public GameObject bloodEffect;
    public GameObject bloodPuddle;
    public AudioClip splatSound;

    [Header("Timing Settings")]
    [Tooltip("Tiempo de espera entre el impacto y la aparición del charco de sangre")]
    public float puddleDelay = 0.4f; // tiempo entre splash y charco

    [Tooltip("Duración de la explosión de sangre (se destruirá después de este tiempo)")]
    public float bloodEffectLifetime = 2f;

    [Tooltip("Duración del charco de sangre antes de desaparecer")]
    public float bloodPuddleLifetime = 5f;

    [Tooltip("Volumen del sonido de impacto")]
    [Range(0f, 1f)]
    public float splatVolume = 1f;

    void Start()
    {
        player = FindFirstObjectByType<TopDownCarController>();

        strafeAmount = Random.Range(-.75f, .75f);
        if (pointsHolder != null)
            pointsHolder.SetParent(null);

        waitCounter = Random.Range(.75f, 1.25f) * pointWaitTime;
    }

    void Update()
    {
        if (isDead) return;

        float yStore = theRB.linearVelocity.y;
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance < chaseRange)
        {
            transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));

            if (distance > stopCloseRange)
            {
                theRB.linearVelocity = (transform.forward + (transform.right * strafeAmount)) * moveSpeed;
                anim.SetBool("Move", true);
            }
            else
            {
                theRB.linearVelocity = Vector3.zero;
                anim.SetBool("Move", false);
            }
        }
        else
        {
            if (patrolPoints.Length > 0)
            {
                if (Vector3.Distance(transform.position, new Vector3(patrolPoints[currentPatrolPoint].position.x, transform.position.y, patrolPoints[currentPatrolPoint].position.z)) < .25f)
                {
                    waitCounter -= Time.deltaTime;
                    theRB.linearVelocity = Vector3.zero;
                    anim.SetBool("Move", false);

                    if (waitCounter <= 0)
                    {
                        currentPatrolPoint++;
                        if (currentPatrolPoint >= patrolPoints.Length)
                            currentPatrolPoint = 0;

                        waitCounter = Random.Range(.75f, 1.25f) * pointWaitTime;
                    }
                }
                else
                {
                    transform.LookAt(new Vector3(patrolPoints[currentPatrolPoint].position.x, transform.position.y, patrolPoints[currentPatrolPoint].position.z));
                    theRB.linearVelocity = transform.forward * moveSpeed;
                    anim.SetBool("Move", true);
                }
            }
            else
            {
                theRB.linearVelocity = Vector3.zero;
                anim.SetBool("Move", false);
            }

            theRB.linearVelocity = new Vector3(theRB.linearVelocity.x, yStore, theRB.linearVelocity.z);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && explode && !isDead)
        {
            StartCoroutine(HandleExplosionSequence());
        }
    }

    private IEnumerator HandleExplosionSequence()
    {
        isDead = true;

        // Desactivar física y colisión
        if (theRB != null)
        {
            theRB.linearVelocity = Vector3.zero;
            theRB.isKinematic = true;
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Ocultar visualmente al animal
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        Quaternion rot = Quaternion.Euler(90f, 0f, 0f);

        // Instancia inmediata del splash
        GameObject splash = Instantiate(bloodEffect, transform.position, rot);
        if (bloodEffectLifetime > 0)
            Destroy(splash, bloodEffectLifetime);

        // Sonido inmediato
        if (splatSound != null)
            AudioSource.PlayClipAtPoint(splatSound, transform.position, splatVolume);

        // Crear charco tras el delay
        yield return new WaitForSeconds(puddleDelay);
        GameObject puddle = Instantiate(bloodPuddle, transform.position, rot);
        if (bloodPuddleLifetime > 0)
            Destroy(puddle, bloodPuddleLifetime);

        // Limpieza de puntos y enemigo (en el frame siguiente)
        if (pointsHolder != null)
            Destroy(pointsHolder.gameObject);

        yield return null; // espera un frame para permitir instanciaciones
        Destroy(gameObject); // ahora sí destruir el enemigo
    }
}
