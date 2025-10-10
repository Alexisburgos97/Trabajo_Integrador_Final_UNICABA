using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private bool _pauseSceneLoaded = false;
    private MenuLoader _menuLoader;

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (!_pauseSceneLoaded)
            {
                // cargar la escena de pausa aditivamente
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadScene("Pausa", LoadSceneMode.Additive);
                _pauseSceneLoaded = true;
            }
            else
            {
                // si ya está cargada -> reanudar
                if (_menuLoader != null)
                {
                    _menuLoader.OnResume();
                    UnloadPauseScene();
                }
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Pausa")
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

    public void UnloadPauseScene()
    {
        if (_pauseSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("Pausa");
            _pauseSceneLoaded = false;
            _menuLoader = null;
        }
    }
}
