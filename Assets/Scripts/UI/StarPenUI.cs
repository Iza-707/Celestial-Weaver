using UnityEngine;
using UnityEngine.UI;

public class StarPenUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject starPenIndicator;
    public Slider lightEnergyGauge;

    private StarPenController starPenController;

    void Start()
    {
        starPenController = FindAnyObjectByType<StarPenController>();

        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (GameManager.Instance == null)
            return;

        bool unlocked = GameManager.Instance.starPenUnlocked;

        if (starPenIndicator != null)
            starPenIndicator.SetActive(unlocked);

        if (lightEnergyGauge != null)
        {
            lightEnergyGauge.gameObject.SetActive(unlocked);

            if (starPenController != null)
            {
                lightEnergyGauge.maxValue =
                    starPenController.maxEnergy;

                lightEnergyGauge.value =
                    starPenController.CurrentEnergy;
            }
        }
    }
}