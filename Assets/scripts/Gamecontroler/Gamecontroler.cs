using UnityEngine;
using UnityEngine.SceneManagement;
/* este es el controlador del juego
 hasta el momento contola la carga de escenas y la velocidad del vehiculo*/
namespace Simplon {

    public  class GameControler : MonoBehaviour
    {
        public static GameControler Instance => instance;
        public static GameControler instance;
        private int _Speed;

        //Variables para manejar las vidas
        [Header("Config. Total de Vidas")]
        [SerializeField] private int _ConfLife=3;
        public int _Life { get; set; }

        ///variables para manejar el combustible, default 200
        [Header("Config Total Combustible")]
        [SerializeField] float _ConfCombustible=200f;
        public float Combustible { get; set; }

        [Header("Setear tiempo del nivel")]
        [SerializeField] private float _tiempo_Restante = 60f; // Temporizador de 60 segundos (puedes ajustar este valor)

        //seteo las cantidad de vueltas por defect a 3
        [SerializeField] private int _total_a_rescatar = 3;

        //guarda cuantos rescate se llevan
        private int _rescate_actual;
       
        //variable que gurda la distancia recorrida
        public float _distancia { get; set; }

        //--------
        /*-------------------------------------------*/
        //--------

        //CONTROLES DE VOLUMEN
        [SerializeField] private float MusicVolume = 0.3f;
        [SerializeField] private float SFXVolume = 0.3f;

        private void Awake()
        {
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.3f);
            SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.3f);

            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else //Instance != null
            {
                Destroy(gameObject);
            }
            
        }
        private void Start()
        {
            //inicializar variables
            ResetVariables();
        }

        //CONTROLES DE AUDIO
        public float GetMusicVolume()
        {
            return MusicVolume;
        }
        public float GetSFXVolume()
        {
            return SFXVolume;
        }

        public void SetMusicVolume(float newVol)
        {
            MusicVolume = newVol;
        }
        public void SetSFXVolume(float newVol)
        {
            SFXVolume = newVol;
        }
        //FIN CONTROLES DE AUDIO


        public float ObtenerMaxFuel()
        {
            //Devuel la configuracionde combustible
            return _ConfCombustible;
        }

        public float Obtener_total_tiempo()
        {
            //devuelve la condiguracion del tiempo del nivel
            return _tiempo_Restante;
        }
        
        public void ResetVariables() {
            //Funcion para renicicializar las variables

            //setear las vidas a 3           
            _Life = _ConfLife;
            
            //setear combustible a 200           
            Combustible = _ConfCombustible;

            //setear vuelta actual a 1
            _rescate_actual = 0;
      
        }
        //cambiar de a la pista2
        public void PasarNivel(string nombre)
        {
            //metodo para cambiar de nivel en el parametro nombre se indica el
            //nombre de la escena a cargar
            SceneManager.LoadScene(nombre);
        }

        public void AgregarnEscena(string nombre)
        {
            //metodo para agregar una escena a la escena actual
            //En el parametro nombre se indica el
            //nombre de la escena a cargar
            SceneManager.LoadScene(nombre, LoadSceneMode.Additive);
        }
        public void ActualizarSpeed(float speed)
        {
            //este metodo mantien actualizada la velocidad del auto
            //el parametro speed recibe un numero flotante y lo alamacena 
            //en la variable speed del GameControler
            _Speed = Mathf.FloorToInt(speed * 100); // como la velocidad estaba en porcentaje multiplico por 100   
        }
        public int ObtnerSpeed()
        {
            //medoto que devuelve la velocidad indicada en la
            //variable Speed
            return _Speed;
        }

        //suma 1 a la variable vuelta actual
        public void Sumar_rescate() => _rescate_actual++;


        public int Obtener_rescate()
        {
            //devuelve cuantos rescatados lleva
            return _rescate_actual;
        }
        public void Setear_total_a_rescatar(int total) { 
            //setea otro valor para el total a rescatar

            _total_a_rescatar=total; 
        }
        public int Obtener_total_a_rescatar() { 
            //devuelve el total de rescates a hacer
            return _total_a_rescatar; 
        }

        public int Obtener_Total_Vidas() {
            //devuelve las vidas disponibles
            return _Life;
        }

        public void Quitar_Vida(int Cantidad) {
            //quita la cantidad de vidas indicadas en el parametro
            //y si llega a 0 rinicia la carrera
            _Life -= Cantidad;
            if (_Life < 1) {
                //perdio y vuelve a reiniciar el juego
                PasarNivel("Perdiste");
                ResetVariables();
            }
        }

        public void ActualizarDistancia(float dist) {
            //guarda la distancia recorrida
            _distancia = dist;
        }

        public void ResetCombustible() {
            //resetear el combustible
            Combustible = _ConfCombustible;
        }
    }


}

