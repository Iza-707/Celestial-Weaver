using UnityEngine;
using UnityEngine.UI;

public class MobileStarPenControls : MonoBehaviour
{
    public static MobileStarPenControls Instance;

    [Header("Buttons")]
    public Button drawButton;
    public Button eraseButton;

    public bool DrawMode { get; private set; }
    public bool EraseMode { get; private set; }

    private void Awake()
    {
        Instance = this;

        DrawMode = false;
        EraseMode = false;
    }

    public void ToggleDraw()
    {
        DrawMode = !DrawMode;

        if (DrawMode)
            EraseMode = false;

        UpdateButtonVisuals();
    }

    public void ToggleErase()
    {
        EraseMode = !EraseMode;

        if (EraseMode)
            DrawMode = false;

        UpdateButtonVisuals();
    }

    private void UpdateButtonVisuals()
    {
        if (drawButton != null)
        {
            ColorBlock colors = drawButton.colors;
            colors.normalColor = DrawMode
                ? Color.white
                : new Color(1f, 1f, 1f, 0.5f);

            drawButton.colors = colors;
        }

        if (eraseButton != null)
        {
            ColorBlock colors = eraseButton.colors;
            colors.normalColor = EraseMode
                ? Color.white
                : new Color(1f, 1f, 1f, 0.5f);

            eraseButton.colors = colors;
        }
    }
}