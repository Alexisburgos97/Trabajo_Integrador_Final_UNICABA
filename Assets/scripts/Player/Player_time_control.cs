using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Simplon;
using UnityEngine.UI;
/*
    la funcion de este script es controlar el tiempo seteado en el gamecontroler
    Lo usa para controlar el tiempo restar vidas si es necesario
*/

public class Player_Time_Control : MonoBehaviour
{
    private float _tiempoRestante = 60f; // Temporizador de 60 segundos (puedes ajustar este valor)
    private float _tiempo_Actual;

    GameControler _controler;

    // Start is called before the first frame update
    void Start()
    {
        _controler = GameControler.Instance;
        _tiempoRestante = _controler.Obtener_total_tiempo();
        _tiempo_Actual = _tiempoRestante;
    }

    // Update is called once per frame
    void Update()
    {
        Actulaizar_Tiempo();
    }

    public void ResetearTiempoVuelta()
    {
        //reestablece el tiempo al los segundos iniciales
        _tiempo_Actual = _tiempoRestante;
    }

    public float Obtenter_tiempo()
    {
        return _tiempo_Actual;
    }

    private void Actulaizar_Tiempo()
    {
        //muestra el tiempo en el hud y si se termina rinicia la carrera
        if (_tiempo_Actual > 0)
        {
            //muestra una cuenta regresiva en el control de texto refernciado
            _tiempo_Actual -= Time.deltaTime;
        }
        else
        {
            _controler.Quitar_Vida(1);
            Reset_TimeControler();
            
        }
    }

    public void Reset_TimeControler()
    {

        _tiempo_Actual = _tiempoRestante;
    }

    public void AddTime(float Seg)
    {
        //sumar tiempo extra
        _tiempo_Actual += Seg;
    }
}

