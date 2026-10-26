using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void OnEnable()
    {
        // This manager persists between scenes, but carried stars belong to
        // one level only. Reloading or changing scenes must clear that state.
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ClearCarriedStar();
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
        CarriedStarID = string.Empty;
    }

    public void ClearCarriedStar()
    {
        CarriedStarID = string.Empty;
    }
}
