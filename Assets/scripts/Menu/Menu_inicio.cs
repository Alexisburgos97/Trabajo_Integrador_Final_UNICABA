using Simplon;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLoader : MonoBehaviour
{
    SoundController soundController;

    bool _isPaused;
   

    [SerializeField] private Button _playButton;
    [SerializeField] private Button _Pausa;
    [SerializeField] private Button _OpcionesButton;
    [SerializeField] private Button _ControlButton;
    [SerializeField] private Button _SonidoButton;
    [SerializeField] private Button _AccesibilidadButton;
    [SerializeField] private Button _CreditoButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _toMainMenuButton;
    [SerializeField] private Button _toNextMundo;

    [Header("Botones a controlar")]
    [SerializeField] private List<Button> botones = new List<Button>();

    [SerializeField] bool _activar_pausa=false;
    [SerializeField] bool _esMenuDePausa = false;  // Nuevo: identifica si este menú fue cargado por PauseManager
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

    public void LoadNextNivel()
    {
        GameControler.Instance.PasarNivel(true);
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
