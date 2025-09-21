using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simplon;
using TMPro;
using System;
using UnityEditor;

public class HUdControl : MonoBehaviour
{
    //t_vueltas es para mostrar el numero de vueltas totales
    //Vuelta_A es para mostrar la vuelta actual
    [SerializeField] private TextMeshProUGUI T_vueltas, Vuelta_A;

    //referencia al texbox para mostrar la distancia, vida, combustible
    [SerializeField] private TextMeshProUGUI VisorDistancia,VisorVida,VisorCombustible;

    //private float tiempoActual;

    //variable para la instancia del gamecontroler
    private GameControler Controler;

    // Start is called before the first frame update
    void Start()
    {
        //Reset_TimeControler();
        Controler = GameControler.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        //muestra en que vuelta esta, en el hud
        Vuelta_A.text = string.Format("{0}", Controler.Obtener_vuelta());
       

        //muestra la cantida vueltas para ganar en el hud
        T_vueltas.text = string.Format("{0}", Controler.Obtener_totalVueltas());
    
        MostrarDistancia();
        //mostrar las vidas disponibles
        MostrarVida();
        //mostrar el combustible disponible
        MostrarCombustible();
    }

    //
    /*-----------------------------------*/
    //

    private void MostrarDistancia() {
        //muestra la distancia recorrida en el hud
        //VisorDistancia.text=Math.Round(Controler.distancia, 2,MidpointRounding.AwayFromZero).ToString();
        VisorDistancia.text = string.Format("{0}m", ((int)Controler.distancia));
      
    }

    private void MostrarVida() {
        VisorVida.text = Controler.Life.ToString();
    }
  
    private void MostrarCombustible() {
        //mostrar el combustible disponible
        VisorCombustible.text = ((int)Controler.Combustible).ToString();
    }

}
