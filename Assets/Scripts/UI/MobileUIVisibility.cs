using UnityEngine;

public class MobileUIVisibility : MonoBehaviour
{
    [SerializeField] private GameObject mobileUI;

    private void Start()
    {
        if (mobileUI == null)
        {
            Debug.LogError("MobileUIVisibility: Mobile UI is NOT assigned!", this);
            return;
        }

#if UNITY_EDITOR
        bool show = TouchSimulationToggle.IsSimulationEnabled;
#else
        bool show = Application.isMobilePlatform;
#endif
        mobileUI.SetActive(show);
        Debug.Log("MobileUIVisibility: mobile UI active = " + show);
    }
}