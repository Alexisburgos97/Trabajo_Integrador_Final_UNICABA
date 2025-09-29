using UnityEngine;
using Simplon;

public class Colectables : MonoBehaviour
{

    public int coins = 0;
    public float fuel = 0;
    public int peopleRescued = 0;
    GameControler _controller;

    void Start()
    {
        _controller = GameControler.Instance;
    }

    public void Collect(Collectible.Type type)
    {
        if (type == Collectible.Type.Coin)
        {
            coins++;
            Debug.Log("Monedas: " + coins);
        }
        else if (type == Collectible.Type.Gasoline)
        {
            fuel += 10f;
            Debug.Log("Gasolina: " + fuel);
        }
        else if (type == Collectible.Type.Person)
        {
            peopleRescued++;
            Debug.Log("Personas rescatadas: " + peopleRescued);
            if ( peopleRescued == _controller.Obtener_totalVueltas())
            {
                //si es la meta y se completaron todas las vueltas pasa de nivel
                _controller.ResetVariables();
                _controller.pasarNivel("Ganaste");
            }
        }
    }
}
