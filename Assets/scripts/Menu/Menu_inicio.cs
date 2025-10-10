using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Simplon;
using UnityEngine.SceneManagement;

public class MenuLoader : MonoBehaviour
{
    //[SerializeField] GameObject _PauseMenu;
    bool _isPaused;

    [SerializeField] private Button _playButton;
    [SerializeField] private Button _Pausa;
    [SerializeField] private Button _controlsButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _toMainMenuButton;
    //[SerializeField] private GameObject controlPanel;
    //[SerializeField] private GameObject hudInfo;





    // Start is called before the first frame update
    private void Awake()
    {
        _playButton?.onClick.AddListener(LoadFirstLevel);
        _controlsButton?.onClick.AddListener(OpenControls);
        _quitButton?.onClick.AddListener(OnQuit);
        _toMainMenuButton?.onClick.AddListener(ToMainMenu);
        _Pausa?.onClick.AddListener(check_pausa);
        
    }

    private void OnDestroy()
    {
        _playButton?.onClick.RemoveAllListeners();
        _controlsButton?.onClick.RemoveAllListeners();
        _quitButton?.onClick.RemoveAllListeners();
        _toMainMenuButton?.onClick.RemoveAllListeners();
        _Pausa?.onClick.RemoveAllListeners();
    }


    // Update is called once per frame
    /*
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (!isPaused) {
                OnPause();
            } else {
                OnResume();
            }
        }

    }
    */
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
        //hudInfo?.gameObject.SetActive(false);//ya no se usa
    }
    public void OpenControls() {
       //controlPanel?.SetActive(true);
    }

    public void LoadFirstLevel() {
        //craga la primer pista de carreras
        GameControler.Instance.PasarNivel("Nivel1");
        GameControler.Instance.AgregarnEscena("Hud");
        //hudInfo?.gameObject.SetActive(true);//ya no se usa
    }
    //anule esta funcion ya que ahora se usa desde el 
    //game manager
    /*public void LoadLevel(int level) {
        SceneManager.LoadScene(level);
    }*/

     public void OnPause() {
        //hudInfo?.gameObject.SetActive(false);//ya no se usa
        //controlPanel?.SetActive(false);
        //PauseMenu?.SetActive(true);
        _isPaused = true;
        Time.timeScale = 0f;

    }
    public void OnResume() {
        print("sin pausa");
        Time.timeScale = 1f;
        //hudInfo?.gameObject.SetActive(true);//ya no se usa
        //PauseMenu?.SetActive(false);
        //controlPanel?.SetActive(false);
        _isPaused = false;
        //mostrar el hud del auto
        //GameControler.instance.agregarnEscena("Hud");
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
