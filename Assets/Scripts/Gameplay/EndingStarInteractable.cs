using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingStarInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string endingSceneName = "EndingScene";

    private Vector3 basePosition;
    private static Sprite cachedStarSprite;

    public string InteractionPrompt => "[E] Follow the Last Constellation";

    private void Start()
    {
        basePosition = transform.position;
    }

    private void Update()
    {
        transform.position = basePosition + Vector3.up * (Mathf.Sin(Time.unscaledTime * 2.0f) * 0.12f);
        transform.Rotate(0f, 0f, 18f * Time.unscaledDeltaTime);
    }

    public void Interact()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.HaveAllCatalogueConstellationsBeenCompletedAndRestored())
        {
            Debug.LogWarning("The Last Constellation is not available yet.");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(endingSceneName);
    }

    public static Sprite CreateStarSprite()
    {
        if (cachedStarSprite != null)
            return cachedStarSprite;

        const int size = 128;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.name = "Last Constellation Star Glow";
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        Vector2 center = new Vector2(0.5f, 0.5f);
        Vector2[] vertices = new Vector2[10];

        for (int i = 0; i < vertices.Length; i++)
        {
            float angle = Mathf.PI * 0.5f + i * Mathf.PI / 5f;
            float radius = (i % 2 == 0) ? 0.43f : 0.19f;
            vertices[i] = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        }

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 point = new Vector2((x + 0.5f) / size, (y + 0.5f) / size);
                float distance = Vector2.Distance(point, center);
                bool insideStar = PointInPolygon(point, vertices);
                Color pixel = Color.clear;

                if (insideStar)
                {
                    float innerGlow = Mathf.Clamp01((0.46f - distance) / 0.25f);
                    pixel = Color.Lerp(
                        new Color(1f, 0.68f, 0.18f, 1f),
                        new Color(1f, 0.98f, 0.78f, 1f),
                        innerGlow * 0.85f
                    );
                }
                else if (distance < 0.5f)
                {
                    float alpha = Mathf.Pow(Mathf.Clamp01((0.5f - distance) / 0.20f), 2f) * 0.38f;
                    pixel = new Color(1f, 0.66f, 0.12f, alpha);
                }

                texture.SetPixel(x, y, pixel);
            }
        }

        texture.Apply(false, true);
        cachedStarSprite = Sprite.Create(
            texture,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            64f
        );
        cachedStarSprite.name = "Last Constellation Star";
        return cachedStarSprite;
    }

    private static bool PointInPolygon(Vector2 point, Vector2[] polygon)
    {
        bool inside = false;
        for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
        {
            bool intersects =
                ((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                (point.x < (polygon[j].x - polygon[i].x) *
                    (point.y - polygon[i].y) /
                    ((polygon[j].y - polygon[i].y) + Mathf.Epsilon) +
                    polygon[i].x);

            if (intersects)
                inside = !inside;
        }
        return inside;
    }
}
