using UnityEngine;
using Simplon;

public class HUD_loader : MonoBehaviour
{
    GameControler _controler;
    [SerializeField] string _escena = "Hud";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _controler = GameControler.Instance;
        _controler.AgregarnEscena(_escena);
        
    }


}
