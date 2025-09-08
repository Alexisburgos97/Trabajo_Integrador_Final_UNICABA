using UnityEngine;

public class vista_camara : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    public Transform player;

    [Header("Configuración de cámara")]
    public float altura = 10f;          // Altura inicial de la cámara
    public float suavizado = 5f;        // Suavizado del movimiento
    public float alturaMin = 5f;        // Altura mínima
    public float alturaMax = 30f;       // Altura máxima
    public float velocidadZoom = 5f;    // Velocidad de cambio de altura

    private Vector3 posicionDeseada;

    void LateUpdate()
    {
        if (player == null) return;

        // Ajustar altura con la rueda del mouse
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            altura -= scroll * velocidadZoom;
            altura = Mathf.Clamp(altura, alturaMin, alturaMax);
        }

        // También se puede controlar con teclas (ejemplo: Q y E)
        if (Input.GetKey(KeyCode.Q)) altura -= velocidadZoom * Time.deltaTime;
        if (Input.GetKey(KeyCode.E)) altura += velocidadZoom * Time.deltaTime;
        altura = Mathf.Clamp(altura, alturaMin, alturaMax);

        // Posición deseada: directamente arriba del jugador
        posicionDeseada = new Vector3(player.position.x, player.position.y + altura, player.position.z);

        // Movimiento suavizado
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, suavizado * Time.deltaTime);

        // Fijar rotación para que siempre mire hacia abajo (cenital pura)
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
