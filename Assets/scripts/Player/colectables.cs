using UnityEngine;
using Simplon;
public class Colectables : MonoBehaviour
{

    public int _coins = 0;
    public float _fuel = 0;
    public int _peopleRescued = 0;
    GameControler _controler;

    void Start()
    {
        _controler = GameControler.Instance;
    }

    public void Collect(Collectible.Type type)
    {
        if (type == Collectible.Type.Coin)
        {
            _coins++;
            Debug.Log("Monedas: " + _coins);
        }
        else if (type == Collectible.Type.Gasoline)
        {
            _fuel += 10f;
            Debug.Log("Gasolina: " + _fuel);
        }
        else if (type == Collectible.Type.Person)
        {
            _peopleRescued++;
            Debug.Log("Personas rescatadas: " + _peopleRescued);
            //sumar rescate
            _controler.Sumar_rescate();
        }
    }
}
