using UnityEngine;
using Simplon;

public class Win_control : MonoBehaviour
{
    [SerializeField] string _Escena_ganar = "Ganaste";
    GameControler _controler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _controler = GameControler.Instance;
        
    }

    // Update is called once per frame
    void Update()
    {
        //controlar si ya se rescataron a todas las personas
        if (_controler.Obtener_rescate() >= _controler.Obtener_total_a_rescatar())
        {
            _controler.PasarNivel(_Escena_ganar);
        }
    }
}
