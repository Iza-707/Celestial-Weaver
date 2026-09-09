using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject selectionStar;
    [SerializeField] private TMP_Text buttonText;

    private Color normalColor;
    private Color hoverColor = Color.white;

    private void Start()
    {
        if (selectionStar != null)
            selectionStar.SetActive(false);

        if (buttonText != null)
            normalColor = buttonText.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectionStar != null)
            selectionStar.SetActive(true);

        if (buttonText != null)
            buttonText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectionStar != null)
            selectionStar.SetActive(false);

        if (buttonText != null)
            buttonText.color = normalColor;
    }
}