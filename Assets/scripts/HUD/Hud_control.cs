using UnityEngine;
using Simplon;
using TMPro;

public class Hud_control : MonoBehaviour
{
    //t_vueltas es para mostrar el numero de vueltas totales
    //Vuelta_A es para mostrar la vuelta actual
    [SerializeField] private TextMeshProUGUI _Rescates,_tot_rescatar;

    //referencia al texbox para mostrar la distancia, vida, combustible
    [SerializeField] private TextMeshProUGUI _VisorDistancia,_VisorVida /*, _VisorCombustible*/;

    private int _vidaTotal;

    //private float tiempoActual;

    //variable para la instancia del gamecontroler
    private GameControler _Controler;

    // Start is called before the first frame update
    void Start()
    {
        //Reset_TimeControler();
        _Controler = GameControler.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        //muestra la cantida de rescatdos / la cantidad a rescatar para ganar en el hud
        _Rescates.text = $"{_Controler.Obtener_rescate()}";
        _tot_rescatar.text = $"{_Controler.Obtener_total_a_rescatar()}";
        //MostrarDistancia();
        //mostrar las vidas disponibles
        MostrarVida();
        //mostrar el combustible disponible
        //MostrarCombustible();
    }

    //
    /*-----------------------------------*/
    //

    private void MostrarDistancia() {
        //muestra la distancia recorrida en el hud
        //VisorDistancia.text=Math.Round(Controler.distancia, 2,MidpointRounding.AwayFromZero).ToString();
        _VisorDistancia.text = string.Format("{0}m", ((int)_Controler._distancia));
      
    }

    private void MostrarVida() {
        // _VisorVida.text = $"{ Controler._Life} / {Controler.Obtener_Total_Vidas()}";
        _vidaTotal = _Controler._Life;
        _VisorVida.text = $"{"0"}{(int)_Controler._Life}";
    }
  /*
    private void MostrarCombustible() {
        //mostrar el combustible disponible
        _VisorCombustible.text = $"{(int)Controler.Combustible} / {(int)Controler.ObtenerMaxFuel()}";
    }
*/
}
