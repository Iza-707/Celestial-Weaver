using UnityEngine;
using UnityEngine.UI;

public class StarPenUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject starPenIndicator;
    public Slider lightEnergyGauge;

    [Header("Energy")]
    public float maxEnergy = 100f;

    private float currentEnergy;

    void Start()
    {
        currentEnergy = maxEnergy;

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

            lightEnergyGauge.maxValue = maxEnergy;
            lightEnergyGauge.value = currentEnergy;
        }
    }
}