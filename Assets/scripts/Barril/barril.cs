using UnityEngine;

public class RollingCylinder : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float fuerzaRotacion = 100f;

    private Rigidbody rb;

    void Start()
    {
        // Obtener el Rigidbody
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Se necesita un Rigidbody en el cilindro!");
            return;
        }

        // Configurar el Rigidbody para mejor física
        rb.mass = 1f;
        rb.linearDamping = 0.1f;
        rb.angularDamping = 0.5f;

        Debug.DrawRay(transform.position, transform.forward * 2, Color.blue, 10f); // Adelante
        Debug.DrawRay(transform.position, transform.right * 2, Color.red, 10f);    // Derecha
        Debug.DrawRay(transform.position, transform.up * 2, Color.green, 10f);     // Arriba
    }

    void FixedUpdate()
    {
        RodarHaciaAdelante();
    }

    void RodarHaciaAdelante()
    {
        // Con rotación X90 y Z90, el cilindro está acostado
        // Para que ruede hacia adelante, necesita rotar en el eje X local

        // Aplicar fuerza hacia adelante (en el eje Z del mundo)
        Vector3 direccionMovimiento = transform.forward;
        rb.AddForce(direccionMovimiento * velocidad, ForceMode.Force);

        // Aplicar torque en el eje X local para que ruede hacia adelante
        // Usamos transform.up porque con las rotaciones aplicadas, 
        // el eje X local ahora apunta en la dirección de transform.up
        Vector3 ejeTorque = transform.up;
        rb.AddTorque(-ejeTorque * fuerzaRotacion * Time.fixedDeltaTime, ForceMode.Force);
    }

    // Método alternativo: movimiento más controlado sin física
    void RodarControlado()
    {
        float distancia = velocidad * Time.fixedDeltaTime;

        // Mover hacia adelante
        transform.position += transform.forward * distancia;

        // Calcular la rotación basada en la distancia recorrida
        // Radio del cilindro (ajustar según tu cilindro)
        float radio = 0.5f;
        float anguloRotacion = (distancia / radio) * Mathf.Rad2Deg;

        // Rotar alrededor del eje correcto
        transform.Rotate(Vector3.right, anguloRotacion, Space.Self);
    }
}

/* 
INSTRUCCIONES DE USO:

1. Crear el cilindro:
   - En Unity, crea un GameObject > 3D Object > Cylinder
   - Rotar en el Inspector: X = 90, Z = 90

2. Agregar componentes:
   - Agregar Rigidbody al cilindro
   - Agregar este script al cilindro

3. Configurar Rigidbody:
   - Mass: 1
   - Drag: 0.1
   - Angular Drag: 0.5
   - Use Gravity: activado
   - Is Kinematic: desactivado

4. Agregar colisión:
   - El cilindro ya tiene un Mesh Collider
   - Asegúrate de que el suelo tenga un Collider

5. Ajustar en el Inspector:
   - Velocidad: controla qué tan rápido se mueve
   - Fuerza Rotación: controla qué tan rápido rueda

NOTA: Si el cilindro no rueda correctamente, puedes descomentar 
el método RodarControlado() y llamarlo en lugar de RodarHaciaAdelante()
para un movimiento más predecible sin física.
*/