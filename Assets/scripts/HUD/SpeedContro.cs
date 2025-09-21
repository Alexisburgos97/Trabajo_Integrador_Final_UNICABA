using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simplon;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
/*este script es el encargado de mostrar la velocidad del vehiculo en la vista del HuD*/
public class SpeedContro : MonoBehaviour
{
    //referencia al text donde se mostrara la velocidad tiene que ser un textmeshpro-textUI
    [SerializeField] private TextMeshProUGUI SpeedViewer;
    [SerializeField]private Slider speedSlider;
     private AudioSource speedAudioSource;
    private void Start() {
        speedAudioSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        mostrarSpeed(); 
    }
    private void mostrarSpeed() {
        //muestra la velocidad 
        SpeedViewer.text = string.Format("{0}", GameControler.instance.ObtnerSpeed());
        speedSlider.value = GameControler.instance.ObtnerSpeed();
        float valorNormalizado = speedSlider.value / 55f; // Normaliza el valor original a 0-1
        float valorInterpolado = Mathf.Lerp(0, 2, valorNormalizado); // Usa Mathf.Lerp para interpolar en el nuevo rango
        float valorFinal = valorInterpolado * 2; // Desnormaliza el valor al rango 0-2
        speedAudioSource.pitch = valorFinal;
    }
}
