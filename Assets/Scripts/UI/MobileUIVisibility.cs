using UnityEngine;

public class MobileUIVisibility : MonoBehaviour
{
    [SerializeField] private GameObject mobileUI;

    private void Update()
    {
#if UNITY_EDITOR
        mobileUI.SetActive(TouchSimulationToggle.IsSimulationEnabled);
#else
        mobileUI.SetActive(true);
#endif
    }
}