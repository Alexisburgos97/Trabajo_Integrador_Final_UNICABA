using UnityEngine;

public class RotacionAleatoriaHijo : MonoBehaviour
{
    [Header("Asignación del objetivo")]
    [SerializeField] private GameObject prefabHijo;

    [Header("Opciones de rotación")]
    [SerializeField] private bool rotarAlInicio = true;
    [SerializeField] private bool rotarEjeX = false;
    [SerializeField] private bool rotarEjeY = true;
    [SerializeField] private bool rotarEjeZ = false;

    void Start()
    {
        if (rotarAlInicio)
            AplicarRotacionAleatoria();
    }

    /// <summary>
    /// Aplica una rotación aleatoria al hijo o prefab asignado
    /// según los ejes habilitados.
    /// </summary>
    public void AplicarRotacionAleatoria()
    {
        GameObject objetivo = null;

        // Usa el prefab asignado o el primer hijo del objeto vacío
        if (prefabHijo != null)
            objetivo = prefabHijo;
        else if (transform.childCount > 0)
            objetivo = transform.GetChild(0).gameObject;

        if (objetivo == null)
        {
            Debug.LogWarning("⚠ No se encontró ningún hijo ni prefab asignado para rotar.", this);
            return;
        }

        // Rotaciones iniciales del objeto
        Vector3 rotacionActual = objetivo.transform.rotation.eulerAngles;

        // Calcula la nueva rotación según los ejes activados
        float rotX = rotarEjeX ? Random.Range(1f, 360f) : rotacionActual.x;
        float rotY = rotarEjeY ? Random.Range(1f, 360f) : rotacionActual.y;
        float rotZ = rotarEjeZ ? Random.Range(1f, 360f) : rotacionActual.z;

        // Asigna la nueva rotación
        objetivo.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
    }
}
