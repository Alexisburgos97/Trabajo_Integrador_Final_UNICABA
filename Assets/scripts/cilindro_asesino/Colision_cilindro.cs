using UnityEngine;
using Simplon;
using System.Collections;

public class Colision_cilindro : MonoBehaviour
{

    [Header("Config. de los pinchos")]
    [SerializeField] float _fuerzaMinPincho = 5f;   // Fuerza mínima
    [SerializeField] float _fuerzaMaxPincho = 15f;  // Fuerza máxima
    [SerializeField] Transform[] _pinchos;           // Lista de pinchos hijos



    
    public void  DispararPinchos()
    {
        if (_pinchos == null) return;

        foreach (var pincho in _pinchos)
        {
            if (pincho == null) continue;                   // ⛔ ya destruido o no asignado
            if (pincho.parent == null) continue;            // ya despadreado en otra llamada

            pincho.SetParent(null, true);                   // ✅ conservar world transform

            // rigidbody
            if (!pincho.TryGetComponent<Rigidbody>(out var rb))
                rb = pincho.gameObject.AddComponent<Rigidbody>();

            float fuerza = Random.Range(_fuerzaMinPincho, _fuerzaMaxPincho);
            rb.AddForce(pincho.forward * fuerza, ForceMode.Impulse);

            // script del pincho
            if (!pincho.TryGetComponent<Pincho>(out var script))
                script = pincho.gameObject.AddComponent<Pincho>();

            script.ActivarPincho();
        }
    }


}
