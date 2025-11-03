using UnityEngine;
using Simplon;
public class Control_Distancia : MonoBehaviour
{
    private Vector3 posicionAnterior;
    private float distanciaTotalRecorrida;
    float control;

    //configura el consumo de combustible para el auto
    [SerializeField] private float ConsumoCombustible=0.2f;

    //variable para la instancia del game manager
    private GameControler Controler;

    void Start()

    {
        // Inicializar la posici�n anterior con la posici�n inicial del objeto
        posicionAnterior = transform.position;
        distanciaTotalRecorrida = 0f;
        control = 0f;
        Controler = GameControler.Instance;
    }

    void Update()
    {
        // Calcular la distancia entre la posici�n anterior y la posici�n actual
        float distancia = Vector3.Distance(posicionAnterior, transform.position);
        control += distancia;

        // Sumar la distancia al total recorrido
        distanciaTotalRecorrida += distancia;

        // Actualizar la posici�n anterior
        posicionAnterior = transform.position;

        //Controler.Distancia = distanciaTotalRecorrida;
        Controler._distancia = distanciaTotalRecorrida;

       if (control > 1 ) {
            //si recorio mas de 1 metro restar combustible
            if (Controler.Combustible > 0)
            {
                QuitarCombustible(ConsumoCombustible);
            }
            else {
                Controler.Quitar_Vida(1);
                Controler.ResetCombustible();
            }
          
            //resetear la variable control
            control = 0;
        }
        // (Opcional) Mostrar la distancia total recorrida en la consola
        //Debug.Log("Distancia total recorrida: " + ((int)distanciaTotalRecorrida));
    }

    // M�todo p�blico para obtener la distancia total recorrida
    public float ObtenerDistanciaTotalRecorrida()
    {
        return distanciaTotalRecorrida;
    }

    //metodo para descontar combustible a medida que se avanza
    private void QuitarCombustible(float restar) { 
        Controler.Combustible-=restar;
    }


}
