using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Simplon;
using UnityEngine.UI;
/*la funcion de este script es*/

public class TimeControl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoTemporizador; // Referencia al objeto TextMeshProUGUI
    [SerializeField] private float tiempoRestante = 60f; // Temporizador de 60 segundos (puedes ajustar este valor)
    [SerializeField] private Slider slider;
    private float tiempoActual;

    // Start is called before the first frame update
    void Start()
    {
        tiempoActual = tiempoRestante;
        slider.maxValue = tiempoRestante;
    }

    // Update is called once per frame
    void Update()
    {
        MostrarTiempo();
        slider.value = tiempoActual;
    }

    public void ResetearTiempoVuelta() {
        //reestablece el tiempo al los segundos iniciales
        tiempoActual = tiempoRestante;
    }

    private void MostrarTiempo()
    {
        //muestra el tiempo en el hud y si se termina rinicia la carrera
        if (tiempoActual > 0)
        {
            //muestra una cuenta regresiva en el control de texto refernciado
            tiempoActual -= Time.deltaTime;
            int minutos = Mathf.FloorToInt(tiempoActual / 60f);
            int segundos = Mathf.FloorToInt(tiempoActual % 60f);
            int centesimas = Mathf.FloorToInt((tiempoActual * 100) % 100);
            textoTemporizador.text = string.Format("{0:00}:{1:00}:{2:00}", minutos, segundos, centesimas);


        }
        else
        {
            textoTemporizador.text = "00:00:00";
            GameControler.Instance.QuitarVida(1);
            Reset_TimeControler();

            //if (GameControler.Instance.Life > 1)
            //{
            //    //si aun tiene vidas se resta 1
            //    //y se resetea el tiempo
            //}
            

        }
    }

    public void Reset_TimeControler()
    {

        tiempoActual = tiempoRestante;
    }

    public void AddTime(float Seg)
    {
        //sumar tiempo extra
        tiempoActual += Seg;
    }
}

