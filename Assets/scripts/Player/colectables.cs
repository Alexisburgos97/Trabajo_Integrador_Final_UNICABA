using UnityEngine;
using Simplon;
public class Colectables : MonoBehaviour
{
    [Header("Sonidos collectables")]
    [SerializeField] AudioSource _coin_sound;
    [SerializeField] AudioSource _Rescate_sound;
    [SerializeField] AudioSource _Fuel_sound;

    [Header("Configurar collectables")]
    [SerializeField] float _cant_fuel = 100f;

    [Header("Visualizar colectables")]
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
            if (_coin_sound != null)
            {
                _coin_sound.Play();
            }
            Debug.Log("Monedas: " + _coins);
        }
        else if (type == Collectible.Type.Gasoline)
        {

            _fuel += _cant_fuel;
            if (_Fuel_sound != null)
            {
                _Fuel_sound.Play();
            }
            //Debug.Log("Gasolina: " + _fuel);
            _controler.Combustible += _cant_fuel;
        }
        else if (type == Collectible.Type.Person)
        {
            _peopleRescued++;
            if (_Rescate_sound != null)
            {
                _Rescate_sound.Play();
            }
            Debug.Log("Personas rescatadas: " + _peopleRescued);
            //sumar rescate
            _controler.Sumar_rescate();
        }
    }
}
