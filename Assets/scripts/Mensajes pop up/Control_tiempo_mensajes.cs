using TMPro;
using UnityEngine;

/*la funcion de este script es mostrar por el hud el tiempo restante para completar el nivel*/

public class Control_tiempo_mensaje : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textoTemporizador; // Referencia al objeto TextMeshProUGUI
    //[SerializeField] private Slider slider;
    float _tiempoActual;
    float _tiempo_total;
    Player_Time_Control _ptc;


    // Start is called before the first frame update
    void Start()
    {
        _ptc = FindAnyObjectByType<Player_Time_Control>();
        _tiempoActual = _ptc.Obtenter_tiempo();
        _tiempo_total = _ptc.Obtenter_tiempo();
        //slider.maxValue = tiempoRestante;
    }

    // Update is called once per frame
    void Update()
    {
        MostrarTiempo();
        //slider.value = tiempoActual;
    }


    private void MostrarTiempo()
    {
        //muestra el tiempo en el hud y si se termina rinicia la carrera
        _tiempoActual = _ptc.Obtenter_tiempo();
        /*
        int minutos = Mathf.FloorToInt(_tiempoActual / 60f);
        int segundos = Mathf.FloorToInt(_tiempoActual % 60f);
        int centesimas = Mathf.FloorToInt(_tiempoActual * 100 % 100);
        */
        int tot_min = (int)_tiempoActual;
        //tiempo en minutos segundos centesimas
        //textoTemporizador.text = $"{minutos}:{segundos}:{centesimas}"; 
        //tiempo en segundos restantes sobre seguntos totales
        _textoTemporizador.text = $"{tot_min}";
    }


}