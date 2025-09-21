using Simplon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlFuelViewer : MonoBehaviour
{
    [SerializeField] Slider slider;
    private GameControler Controller;
    // Start is called before the first frame update
    void Start()
    {
        Controller= GameControler.Instance;
        slider.maxValue = Controller.ObtenerMaxFuel();
       
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = Controller.Combustible;
    }


}
