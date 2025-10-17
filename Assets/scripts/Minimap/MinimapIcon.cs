using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    [Tooltip("Prefab con SpriteRenderer en Layer 'Minimap' (tu PNG).")]
    public GameObject iconPrefab;

    [Tooltip("Altura absoluta para que no lo tape el mundo en la cámara del minimapa.")]
    public float height = 75f;

    [Tooltip("Rota el icono copiando el yaw del objeto (útil si el PNG es flecha).")]
    public bool rotateWithTarget = true;

    Transform icon;

    void OnEnable()
    {
        if (!iconPrefab) return;
        icon = Instantiate(iconPrefab).transform;
        SetLayerRecursive(icon.gameObject, LayerMask.NameToLayer("Minimap"));
        icon.SetParent(null, false);
        UpdateIcon();
    }

    void LateUpdate()
    {
        if (icon) UpdateIcon();
    }

    void OnDisable()
    {
        if (icon) Destroy(icon.gameObject);
    }

    void UpdateIcon()
    {
        var p = transform.position; p.y += height;
        icon.position = p;
        icon.rotation = rotateWithTarget
            ? Quaternion.Euler(90f, transform.eulerAngles.y, 0f)
            : Quaternion.Euler(90f, 0f, 0f);
    }

    static void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform c in go.transform) SetLayerRecursive(c.gameObject, layer);
    }
}