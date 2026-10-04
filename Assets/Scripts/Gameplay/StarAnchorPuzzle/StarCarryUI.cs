using UnityEngine;
using TMPro;

public class StarCarryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI holdingText;

    private void Update()
    {
        if (StarCarryManager.Instance == null)
            return;

        if (StarCarryManager.Instance.IsCarryingStar)
        {
            holdingText.gameObject.SetActive(true);

            holdingText.text =
                "HOLDING A STAR: " +
                StarCarryManager.Instance.CarriedStarID.ToUpper();
        }
        else
        {
            holdingText.gameObject.SetActive(false);
        }
    }
}