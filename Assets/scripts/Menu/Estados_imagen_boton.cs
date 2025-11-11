
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Estados_imagen_boton: MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Sprites del botón")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite clickSprite;

    [Header("Referencia al objeto con la imagen base")]
    [SerializeField] private Image baseImage; // Asignar BaseBotonMenu si no se detecta automáticamente

    [Header("Texto del botón (TMP)")]
    [SerializeField] private TMP_Text buttonText;

    [Header("Colores del texto por estado")]
    [SerializeField] private Color normalTextColor = new Color(253f/255f,207f/255f,137f/255f,255f/255f);
    [SerializeField] private Color hoverTextColor = Color.yellow;
    [SerializeField] private Color clickTextColor = Color.gray;

    private void Start()
    {
        // Buscar automáticamente los objetos si no se asignaron
        if (baseImage == null)
        {
            Transform baseTransform = transform.Find("BaseBotonMenu");
            if (baseTransform != null)
                baseImage = baseTransform.GetComponent<Image>();
        }

        if (buttonText == null)
        {
            buttonText = GetComponentInChildren<TMP_Text>();
        }

        // Estado inicial
        if (baseImage != null && normalSprite != null)
            baseImage.sprite = normalSprite;

        if (buttonText != null)
            buttonText.color = normalTextColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (baseImage != null && hoverSprite != null)
            baseImage.sprite = hoverSprite;

        if (buttonText != null)
            buttonText.color = hoverTextColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (baseImage != null && normalSprite != null)
            baseImage.sprite = normalSprite;

        if (buttonText != null)
            buttonText.color = normalTextColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (baseImage != null && clickSprite != null)
            baseImage.sprite = clickSprite;

        if (buttonText != null)
            buttonText.color = clickTextColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (baseImage != null && hoverSprite != null)
            baseImage.sprite = hoverSprite;

        if (buttonText != null)
            buttonText.color = hoverTextColor;
    }
}