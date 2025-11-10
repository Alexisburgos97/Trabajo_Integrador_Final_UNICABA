using Simplon;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLoader : MonoBehaviour
{
    //SOUND CONTROLLER
    SoundController soundController;

    //SOUNDS
    [SerializeField] private AudioClip Music;
    //[SerializeField] private AudioClip ClickSound;

    //[SerializeField] GameObject _PauseMenu;
    bool _isPaused;

    [SerializeField] private Button _playButton;
    [SerializeField] private Button _Pausa;
    [SerializeField] private Button _controlsButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _toMainMenuButton;

    [Header("Botones a controlar")]
    [SerializeField] private List<Button> botones = new List<Button>();

    // Start is called before the first frame update
    private void Awake()
    {
        _playButton?.onClick.AddListener(LoadFirstLevel);
        _controlsButton?.onClick.AddListener(OpenControls);
        _quitButton?.onClick.AddListener(OnQuit);
        _toMainMenuButton?.onClick.AddListener(ToMainMenu);
        _Pausa?.onClick.AddListener(check_pausa);
    }

    private void Start()
    {
        soundController = GetComponent<SoundController>();
        soundController.PlayMusic(Music);
    }

    private void OnDestroy()
    {
        _playButton?.onClick.RemoveAllListeners();
        _controlsButton?.onClick.RemoveAllListeners();
        _quitButton?.onClick.RemoveAllListeners();
        _toMainMenuButton?.onClick.RemoveAllListeners();
        _Pausa?.onClick.RemoveAllListeners();
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
        // avisar al PauseManager que descargue la escena
        FindFirstObjectByType<PauseManager>()?.UnloadPauseScene();
    }
}
    public void ToMainMenu() {
        //carga el menu
        GameControler.Instance.PasarNivel("Inicio");
        GameControler.Instance.ResetVariables();
        Time.timeScale = 1.0f;
    }
    public void OpenControls() {
        GameControler.Instance.PasarNivel("Opciones");
    }

    public void LoadFirstLevel() {
        //craga la primer pista de carreras
        GameControler.Instance.PasarNivel("Nivel1");
        GameControler.Instance.AgregarnEscena("Hud");
    }

     public void OnPause() {
        _isPaused = true;
        Time.timeScale = 0f;
    }

    public void OnResume() {
        print("sin pausa");
        Time.timeScale = 1f;
        _isPaused = false;
    }
    public void OnQuit() {
       Application.Quit();
        #if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
        #endif
    }
 

   void UnloadPauseScene()
    {
      SceneManager.UnloadSceneAsync("Pausa");
   
    }
}
