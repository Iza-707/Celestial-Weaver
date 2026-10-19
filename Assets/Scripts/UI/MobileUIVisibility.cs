
using UnityEngine;

public class MobileUIVisibility : MonoBehaviour
{
    [SerializeField] private GameObject mobileUI;

    private void Start()
    {
        if (mobileUI == null)
        {
            Debug.LogWarning("MobileUIVisibility: Mobile UI is not assigned.", this);
            return;
        }

#if UNITY_EDITOR
        mobileUI.SetActive(TouchSimulationToggle.IsSimulationEnabled);
#else
        mobileUI.SetActive(Application.isMobilePlatform);
#endif
    }
}
