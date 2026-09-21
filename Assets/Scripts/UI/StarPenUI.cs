using UnityEngine;
using UnityEngine.UI;

public class StarPenUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject starPenIndicator_Draw;
    public GameObject starPenIndicator_Erase;
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

        if (starPenIndicator_Draw != null)
            starPenIndicator_Draw.SetActive(unlocked);

        if (starPenIndicator_Erase != null)
            starPenIndicator_Erase.SetActive(unlocked);

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