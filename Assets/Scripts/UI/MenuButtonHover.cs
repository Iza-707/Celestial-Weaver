using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject selectionStar;

    private void Start()
    {
        if (selectionStar != null)
            selectionStar.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectionStar != null)
            selectionStar.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectionStar != null)
            selectionStar.SetActive(false);
    }
}