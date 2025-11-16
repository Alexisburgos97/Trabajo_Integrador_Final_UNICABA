using UnityEngine;
using Simplon;
using UnityEngine.SceneManagement;
public class Win_spot : MonoBehaviour
{
    [SerializeField] string _Menu_Nivel_ganado="Nivel_ganado";
    [SerializeField] string _Menu_nivel_70="Nivel_ganado_70";
    GameControler _controler;
    private MenuLoader _menuLoader;
    void Start()
    {
        _controler=GameControler.Instance;
    }

    void OnTriggerEnter(Collider other)
    {
        int rescates = _controler.Obtener_rescate();
        int total = _controler.Obtener_total_a_rescatar();
        float porcentaje = (float)rescates / total;
        if (other.CompareTag("Player"))
        {
            if(porcentaje >= 0.70f)
            {
                //carga la escena en modo aditivo y activa la pausa
                _controler.AgregarnEscena(_Menu_nivel_70);
            }
            else if(rescates < total)
            {
                //carga la escena en modo sibgle
                _controler.mostrar_menu(_Menu_Nivel_ganado);
            }
                   
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == _Menu_nivel_70)
        {
            // buscar el MenuLoader en la escena recién cargada
            var rootObjects = scene.GetRootGameObjects();
            foreach (var obj in rootObjects)
            {
                _menuLoader = obj.GetComponentInChildren<MenuLoader>();
                if (_menuLoader != null)
                    break;
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;

            // arrancar en pausa
            _menuLoader?.OnPause();
        }
    }
}
