using Simplon;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Constantes;
public class MenuLoader : MonoBehaviour
{
    SoundController soundController;

    bool _isPaused;
   

    [SerializeField] private Button _playButton;
    [SerializeField] private Button _Niveles;
    [SerializeField] private Button _Pausa;
    [SerializeField] private Button _OpcionesButton;
    [SerializeField] private Button _ControlButton;
    [SerializeField] private Button _SonidoButton;
    [SerializeField] private Button _AccesibilidadButton;
    [SerializeField] private Button _CreditoButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _toMainMenuButton;
    [SerializeField] private Button _toNextMundo;
    [Header("Niveles")]
    [SerializeField] private Button _to_Nivel_1;
    [SerializeField] private Button  _to_Nivel_2;
    [SerializeField] private Button  _to_Nivel_3;
    [SerializeField] private Button  _to_Nivel_4;
    [SerializeField] private Button _to_Nivel_5;
    [SerializeField] bool _activar_pausa=false;
    [Header("Activar si el menu pausa")]
    [SerializeField] bool _esMenuDePausa = false;  // identifica si este menú fue cargado por PauseManager
    private void Awake()
    {
        _playButton?.onClick.AddListener(LoadFirstLevel);
        _OpcionesButton?.onClick.AddListener(OpenOpciones);
        _ControlButton?.onClick.AddListener(OpenControles);
        _SonidoButton?.onClick.AddListener(OpenSonido);
        _AccesibilidadButton?.onClick.AddListener(OpenAccesibilidad);
        _CreditoButton?.onClick.AddListener(OpenCretido);
        _quitButton?.onClick.AddListener(OnQuit);
        _toMainMenuButton?.onClick.AddListener(ToMainMenu);
        _Pausa?.onClick.AddListener(check_pausa);
        _toNextMundo?.onClick.AddListener(ToNextMundo);
        _Niveles?.onClick.AddListener(Mostrar_niveles);
        //niveles
        _to_Nivel_1?.onClick.AddListener(Al_nivel_1);
        _to_Nivel_2?.onClick.AddListener(Al_nivel_2);
        _to_Nivel_3?.onClick.AddListener(Al_nivel_3);
        _to_Nivel_4?.onClick.AddListener(Al_nivel_4);
        _to_Nivel_5?.onClick.AddListener(Al_nivel_5);

        // Detecta si esta escena ES la de pausa
        if (SceneManager.GetSceneAt(SceneManager.sceneCount - 1).name == "Pausa")
            _esMenuDePausa = true;
    }

    private void OnDestroy()
    {
        _playButton?.onClick.RemoveAllListeners();
        _OpcionesButton?.onClick.RemoveAllListeners();
        _ControlButton?.onClick.RemoveAllListeners();
        _SonidoButton?.onClick.RemoveAllListeners();
        _AccesibilidadButton?.onClick.RemoveAllListeners();
        _CreditoButton?.onClick.RemoveAllListeners();
        _quitButton?.onClick.RemoveAllListeners();
        _toMainMenuButton?.onClick.RemoveAllListeners();
        _Pausa?.onClick.RemoveAllListeners();
        _toNextMundo?.onClick.RemoveAllListeners();
        _Niveles?.onClick.RemoveAllListeners();
        //niveles
        _to_Nivel_1?.onClick.RemoveAllListeners();
        _to_Nivel_2?.onClick.RemoveAllListeners();
        _to_Nivel_3?.onClick.RemoveAllListeners();
        _to_Nivel_4?.onClick.RemoveAllListeners();
        _to_Nivel_5?.onClick.RemoveAllListeners();

    }

    void Start()
    {
        // Solo los menús de Opciones / Controles / etc. usan esto
        if (_activar_pausa)
            OnPause();
    }

    public void check_pausa()
    {
        if (!_isPaused)
        {
            OnPause();
        }
        else
        {
            OnResume();

            if (_esMenuDePausa)
            {
                // Usar PauseManager
                FindFirstObjectByType<PauseManager>()?.UnloadPauseScene();
            }
            else
            {
                // No es menu de pausa -> descargar la escena actual del menú
                Scene actual = gameObject.scene;
                SceneManager.UnloadSceneAsync(actual);
            }
        }
    }

    public void ToMainMenu()
    {
        GameControler.Instance.mostrar_menu_inicial();
        GameControler.Instance.ResetVariables();
        Time.timeScale = 1.0f;
    }

    public void Mostrar_niveles()
    {
        GameControler.Instance.mostrar_menu("Niveles");
    }
    public void OpenOpciones()
    {
        GameControler.Instance.mostrar_menu("Opciones");
    }

    public void OpenControles()
    {
        GameControler.instance.mostrar_menu("Controles");
    }

    public void OpenSonido()
    {
        GameControler.instance.mostrar_menu("Sonido");
    }

    public void OpenAccesibilidad()
    {
        // Implementación futura
    }

    public void OpenCretido()
    {
        GameControler.instance.mostrar_menu("Creditos");
    }

    public void ToNextMundo()
    {
        if (_activar_pausa)
        {
            OnResume();
        }
        GameControler.instance.PasarNivel(true);
    }

    public void LoadFirstLevel()
    {
        GameControler.Instance.PasarNivel();
    }

    //cargar niveles
    public void Al_nivel_1()
    {
        GameControler.Instance.cargar_nivel(_NIVEL_1);
    }
    public void Al_nivel_2()
    {
        GameControler.Instance.cargar_nivel(_NIVEL_2);
    }
    public void Al_nivel_3()
    {
        GameControler.Instance.cargar_nivel(_NIVEL_3);
    }
    public void Al_nivel_4()
    {
        GameControler.Instance.cargar_nivel(_NIVEL_4);
    }
    public void Al_nivel_5()
    {
        GameControler.Instance.cargar_nivel(_NIVEL_5);
    }

    public void OnPause()
    {
        _isPaused = true;
        Time.timeScale = 0f;
    }

    public void OnResume()
    {
        Time.timeScale = 1f;
        _isPaused = false;
    }

    public void OnQuit()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
    }
}
