using UnityEngine;

public class AnimalRunPatrolController : MonoBehaviour
{
    private TopDownCarController player; // Referencia al objeto del jugador

    public float moveSpeed; // Velocidad de movimiento del enemigo

    public Rigidbody theRB; // Rigidbody reference

    public Animator anim; // Animator reference

    // Persecucion
    public float chaseRange = 15f, stopCloseRange = 4f; // Rango de persecución y rango de parada del enemigo

    // Patrullaje
    private float strafeAmount; // Cantidad de movimiento lateral aleatorio del enemigo
    public Transform[] patrolPoints; // Puntos de patrullaje del enemigo
    [HideInInspector] public int currentPatrolPoint; // Índice del punto de patrullaje actual del enemigo
    public Transform pointsHolder; // Objeto padre que contiene los puntos de patrullaje
    public float pointWaitTime = 3f; // Tiempo de espera en cada punto de patrullaje
    private float waitCounter; // Contador de espera en cada punto de patrullaje


    public float waitToDisappear = 3f; // Tiempo de espera para desaparecer después de morir

    public bool explode; // Indica si el enemigo explota al contacto
    private bool isDead;
    public GameObject bloodEffect; // Prefab de la explosión al tocar al jugador
    public GameObject bloodPuddle; // Prefab de mancha de sangre

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<TopDownCarController>(); // Encontrar el objeto del jugador en la escena

        // Patrullaje
        strafeAmount = Random.Range(-.75f, .75f); // Cantidad de movimiento lateral aleatorio del enemigo
        pointsHolder.SetParent(null); // Obtener el objeto padre que contiene los puntos de patrullaje
        waitCounter = Random.Range(.75f, 1.25f) * pointWaitTime; // Inicializar el contador de espera en un rango aleatorio entre 0.75 y 1.25 segundos

    }

    // Update is called once per frame
    void Update()
    {
        if (isDead == true)
        {
            waitToDisappear -= Time.deltaTime; // Reducir el contador de espera para desaparecer

            if (waitToDisappear <= 0) // Si el contador ha llegado a cero
            {
                // se reduce la escala del enemigo
                transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime); // Hacer que el enemigo desaparezca lentamente

                if (transform.localScale.x <= .1f) // Si la escala del enemigo es menor o igual a 0.1
                {
                    Destroy(gameObject); // Destruir el objeto del enemigo
                }
            }
            return; // Si el enemigo está muerto, no hacer nada y termina el update
        }

        float yStore = theRB.linearVelocity.y; // Almacenar la velocidad en el eje Y del Rigidbody

        float distance = Vector3.Distance(transform.position, player.transform.position); // Calcular la distancia entre el enemigo y el jugador

        if (distance < chaseRange) // Si el jugador está dentro del rango de persecución y el jugador no esa muerto
        {
            //transform.LookAt(player.transform.position);
            transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z)); // Mirar al jugador en el eje Y

            if (distance > stopCloseRange) // Si el jugador esta en el rango de parada
            {
                theRB.linearVelocity = (transform.forward + (transform.right * strafeAmount)) * moveSpeed; // Movimiento del enemigo hacia el jugador con movimiento lateral aleatorio

                anim.SetBool("Move", false); // Hacer que la animación de caminar se reproduzca cuando el enemigo se mueva
                anim.SetBool("Run", true); // 
                
            }
            else
            {
                theRB.linearVelocity = Vector3.zero; // Detener el movimiento del enemigo cuando está cerca del jugador

                anim.SetBool("Run", false); // Hacer que la animación de caminar se detenga cuando el enemigo no se mueva
            }
        }
        else
        {

            if (patrolPoints.Length > 0) // Si hay puntos de patrullage definidos
            {
                if (Vector3.Distance(transform.position, new Vector3(patrolPoints[currentPatrolPoint].position.x, transform.position.y, patrolPoints[currentPatrolPoint].position.z)) < .25f) // Si el enemigo está cerca del punto de patrullaje actual
                {
                    // Reducir el contador y detener el movimiento
                    waitCounter -= Time.deltaTime; // Reducir el contador de espera
                    theRB.linearVelocity = Vector3.zero; // Detener el movimiento del enemigo
                    anim.SetBool("Move", false); // Hacer que la animación de caminar se detenga cuando el enemigo no se mueva

                    if (waitCounter <= 0) // Si el contador de espera ha llegado a cero
                    {
                        // Cambiar al siguiente punto de patrullaje
                        currentPatrolPoint++; // Cambiar al siguiente punto de patrullaje
                        if (currentPatrolPoint >= patrolPoints.Length) // Si se ha llegado al último punto de patrullaje
                        {
                            currentPatrolPoint = 0; // Volver al primer punto de patrullaje
                        }

                        waitCounter = Random.Range(.75f, 1.25f) * pointWaitTime; // Reiniciar el contador de espera en un rango aleatorio entre 0.75 y 1.25 segundos
                    }

                }
                else
                {
                    transform.LookAt(new Vector3(patrolPoints[currentPatrolPoint].position.x, transform.position.y, patrolPoints[currentPatrolPoint].position.z)); // Mirar al siguiente punto de patrullaje

                    theRB.linearVelocity = transform.forward * moveSpeed; // Movimiento del enemigo hacia el siguiente punto de patrullaje

                    anim.SetBool("Move", true); // Hacer que la animación de caminar se reproduzca cuando el enemigo se mueva     
                }
            }
            else
            {
                theRB.linearVelocity = Vector3.zero; // Detener el movimiento del enemigo cuando está fuera del rango de persecución

                anim.SetBool("Move", false); // Hacer que la animación de caminar se detenga cuando el enemigo no se mueva
            }

            theRB.linearVelocity = new Vector3(theRB.linearVelocity.x, yStore, theRB.linearVelocity.z); // Mantener la velocidad en el eje Y

        }
    }

    // Explota al tocar al jugador
    void OnCollisionEnter(Collision other)
    {
        // Verificar si el objeto con el que colisiona es el jugador y si el enemigo está configurado para explotar
        if (other.gameObject.CompareTag("Player") && explode == true)
        {
            // Quaternion hitRot = Quaternion.LookRotation(Vector3.up);
            // Instanciar efectos de sangre con rotación corregida
            Quaternion rot = Quaternion.Euler(90f, 0f, 0f);
            //Quaternion rot = Quaternion.Euler(90f, Random.Range(0f, 360f), 0f);
            Instantiate(bloodEffect, transform.position, rot);
            Instantiate(bloodPuddle, transform.position, rot);

            // Debug.Log("Enemy hit"); // Imprimir un mensaje en la consola cuando el enemigo recibe daño

            Destroy(gameObject); // Destruir el objeto del enemigo
            
            theRB.linearVelocity = Vector3.zero; // Detener el movimiento del enemigo
            theRB.isKinematic = true; // Hacer que el Rigidbody sea cinemático para que no se vea afectado por la física



            GetComponent<Collider>().enabled = false; // Desactivar el collider del enemigo
            isDead = true;

        }
    }
}
