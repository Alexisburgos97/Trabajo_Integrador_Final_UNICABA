
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Estados_imagen_boton: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Imágenes del estado del botón")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite clickSprite;

    [Header("Referencia al objeto con la imagen base")]
    [SerializeField] private Image baseImage; // Asigna aquí tu BaseBotonMenu

    private void Start()
    {
        if (baseImage == null)
        {
            // Intentar encontrar automáticamente la imagen "BaseBotonMenu" si no se asignó
            Transform baseTransform = transform.Find("BaseBotonMenu");
            if (baseTransform != null)
                baseImage = baseTransform.GetComponent<Image>();
        }

        if (baseImage != null && normalSprite != null)
            baseImage.sprite = normalSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (baseImage != null && hoverSprite != null)
            baseImage.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (baseImage != null && normalSprite != null)
            baseImage.sprite = normalSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (baseImage != null && clickSprite != null)
            baseImage.sprite = clickSprite;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (baseImage != null && hoverSprite != null)
            baseImage.sprite = hoverSprite;
    }
}
