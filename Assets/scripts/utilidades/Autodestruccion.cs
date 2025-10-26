using UnityEngine;

public class Autodestruccion : MonoBehaviour
{
    [Header("Tiempo para autodestruirse")]
    [SerializeField] float _tiempo = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, _tiempo);
    }

}
