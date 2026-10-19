using UnityEngine;

public class StarCarryManager : MonoBehaviour
{
    public static StarCarryManager Instance { get; private set; }

    public string CarriedStarID { get; private set; }

    public bool IsCarryingStar =>
        !string.IsNullOrEmpty(CarriedStarID);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PickUpStar(string starID)
    {
        if (IsCarryingStar)
            return;

        CarriedStarID = starID;

        Debug.Log("Picked up star: " + CarriedStarID);
    }

    public void ClearStar()
    {
        Debug.Log("Placed star: " + CarriedStarID);

        CarriedStarID = "";
    }

    public void ClearCarriedStar()
    {
        CarriedStarID = "";
    }
}