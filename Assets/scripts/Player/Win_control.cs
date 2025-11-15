using UnityEngine;
using Simplon;

public class Win_control : MonoBehaviour
{
    [SerializeField] string _Escena_ganar = "Ganaste";
    [SerializeField] private GameObject _canvas70;   // Canvas con botones
    private bool _mostrado70 = false;

    GameControler _controler;

    void Start()
    {
        _controler = GameControler.Instance;
        if (_canvas70 != null)
            _canvas70.SetActive(false);
    }

    void Update()
    {
        int rescates = _controler.Obtener_rescate();
        int total = _controler.Obtener_total_a_rescatar();

        if (total <= 0) return;

        float porcentaje = (float)rescates / total;

        // ✔️ 1) Mostrar opción del 70% una sola vez
        if (!_mostrado70 && porcentaje >= 0.70f && rescates < total)
        {
            _mostrado70 = true;
            if (_canvas70 != null)
                _canvas70.SetActive(true);
        }

        // ✔️ 2) Ganar directamente cuando llega al 100%
        if (rescates >= total)
        {
            //_controler.PasarNivel(true);
        }
    }

    // ✔️ Método llamado por botón "Finalizar ahora"
    public void FinalizarNivel()
    {
        //_controler.PasarNivel(true);
    }

    // ✔️ Método llamado por botón "Seguir rescatando"
    public void Continuar()
    {
        if (_canvas70 != null)
            _canvas70.SetActive(false);
    }
}
