using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Simplon;
using UnityEngine.Events; //nameSpace del game manager

public class MenuLoader : MonoBehaviour
{
    [SerializeField] GameObject PauseMenu;
    bool isPaused;

    [SerializeField] private Button playButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button toMainMenuButton;
    [SerializeField] private GameObject controlPanel;
    //[SerializeField] private GameObject hudInfo;





    // Start is called before the first frame update
    private void Awake() {
        playButton?.onClick.AddListener(LoadFirstLevel);
        controlsButton?.onClick.AddListener(OpenControls);
        quitButton?.onClick.AddListener(OnQuit);
        toMainMenuButton?.onClick.AddListener(ToMainMenu);
        
    }

    private void OnDestroy() {
        playButton?.onClick.RemoveAllListeners();
        controlsButton?.onClick.RemoveAllListeners();
        quitButton?.onClick.RemoveAllListeners();
        toMainMenuButton?.onClick.RemoveAllListeners();
    }


    // Update is called once per frame
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
    public void ToMainMenu() {
        //carga el menu
        GameControler.Instance.pasarNivel("MenuScene");
        Time.timeScale= 1.0f;
        //hudInfo?.gameObject.SetActive(false);//ya no se usa
    }
    public void OpenControls() {
       controlPanel?.SetActive(true);
    }

    public void LoadFirstLevel() {
        //craga la primer pista de carreras
        GameControler.Instance.pasarNivel("Nivel1");
        //GameControler.Instance.agregarnEscena("Hud_V2.0");
        //hudInfo?.gameObject.SetActive(true);//ya no se usa
    }
    //anule esta funcion ya que ahora se usa desde el 
    //game manager
    /*public void LoadLevel(int level) {
        SceneManager.LoadScene(level);
    }*/

    void OnPause() {
        //hudInfo?.gameObject.SetActive(false);//ya no se usa
        controlPanel?.SetActive(false);
        PauseMenu?.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f;

    }
    public void OnResume() {
        print("sin pausa");
        Time.timeScale = 1f;
        //hudInfo?.gameObject.SetActive(true);//ya no se usa
        PauseMenu?.SetActive(false);
        controlPanel?.SetActive(false);
        isPaused = false;
        //mostrar el hud del auto
        //GameControler.instance.agregarnEscena("Hud");
    }
    public void OnQuit() {
       Application.Quit();
        #if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
        #endif
    }
}
