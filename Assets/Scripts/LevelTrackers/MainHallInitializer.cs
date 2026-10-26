using UnityEngine;

public class MainHallInitializer : MonoBehaviour
{
    [Header("Ending Star")]
    [Tooltip("World position used when all nine catalogue constellations are restored and completed.")]
    [SerializeField] private Vector3 endingStarSpawnPosition = new Vector3(3.15f, -1.0f, 0f);
    [SerializeField] private float endingStarScale = 1.1f;

    private GameObject endingStar;

    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.hasReachedMainHall = true;

        TrySpawnEndingStar();
    }

    private void Update()
    {
        // The last level may have been completed immediately before the
        // player returned to the hub, so keep checking while in Main Hall.
        TrySpawnEndingStar();
    }

    private void TrySpawnEndingStar()
    {
        if (endingStar != null ||
            GameManager.Instance == null ||
            !GameManager.Instance.HaveAllCatalogueConstellationsBeenCompletedAndRestored())
        {
            return;
        }

        endingStar = new GameObject("The Last Constellation");
        endingStar.transform.position = endingStarSpawnPosition;
        endingStar.transform.localScale = Vector3.one * endingStarScale;
        endingStar.layer = LayerMask.NameToLayer("Interactable");

        SpriteRenderer renderer = endingStar.AddComponent<SpriteRenderer>();
        renderer.sprite = EndingStarInteractable.CreateStarSprite();
        renderer.sortingLayerName = "FallenStar";
        renderer.sortingOrder = 50;

        CircleCollider2D interactionCollider = endingStar.AddComponent<CircleCollider2D>();
        interactionCollider.isTrigger = true;
        interactionCollider.radius = 0.55f;

        endingStar.AddComponent<EndingStarInteractable>();

        Debug.Log("All catalogue constellations completed and restored. The Last Constellation has appeared in the Main Hall.");
    }
}
