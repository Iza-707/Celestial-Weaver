using UnityEngine;

public class MainHallInitializer : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.hasReachedMainHall = true;
        }
    }
}