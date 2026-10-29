
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
        if (Instance == null)
        {
            Instance = this;
            DrawMode = false;
            EraseMode = false;
        }
        else if (Instance != this)
        {
            Instance.ConfigureButtons(drawButton, eraseButton);
            return;
        }

        ConfigureButtons(drawButton, eraseButton);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void ConfigureButtons(Button draw, Button erase)
    {
        if (Instance != null && Instance != this)
        {
            Instance.ConfigureButtons(draw, erase);
            return;
        }

        if (draw != null)
            drawButton = draw;

        if (erase != null)
            eraseButton = erase;

        if (drawButton != null && drawButton.onClick.GetPersistentEventCount() == 0)
            drawButton.onClick.AddListener(ToggleDraw);

        if (eraseButton != null && eraseButton.onClick.GetPersistentEventCount() == 0)
            eraseButton.onClick.AddListener(ToggleErase);

        UpdateButtonVisuals();
    }

    public void ToggleDraw()
    {
        if (Instance != null && Instance != this)
        {
            Instance.ToggleDraw();
            return;
        }

        SelectDrawMode(!DrawMode);
    }

    public void ToggleErase()
    {
        if (Instance != null && Instance != this)
        {
            Instance.ToggleErase();
            return;
        }

        SelectEraseMode(!EraseMode);
    }

    public void SelectDrawMode(bool enabled = true)
    {
        if (Instance != null && Instance != this)
        {
            Instance.SelectDrawMode(enabled);
            return;
        }

        DrawMode = enabled;
        if (enabled)
            EraseMode = false;

        UpdateButtonVisuals();
    }

    public void SelectEraseMode(bool enabled = true)
    {
        if (Instance != null && Instance != this)
        {
            Instance.SelectEraseMode(enabled);
            return;
        }

        EraseMode = enabled;
        if (enabled)
            DrawMode = false;

        UpdateButtonVisuals();
    }

    private void UpdateButtonVisuals()
    {
        if (drawButton != null)
        {
            ColorBlock colors = drawButton.colors;
            Color modeColor = DrawMode
                ? Color.white
                : new Color(1f, 1f, 1f, 0.5f);

            colors.normalColor = modeColor;
            colors.highlightedColor = modeColor;
            colors.pressedColor = modeColor;
            colors.selectedColor = modeColor;
            drawButton.colors = colors;
        }

        if (eraseButton != null)
        {
            ColorBlock colors = eraseButton.colors;
            Color modeColor = EraseMode
                ? Color.white
                : new Color(1f, 1f, 1f, 0.5f);

            colors.normalColor = modeColor;
            colors.highlightedColor = modeColor;
            colors.pressedColor = modeColor;
            colors.selectedColor = modeColor;
            eraseButton.colors = colors;
        }
    }

    public bool IsDrawMode()
    {
        return DrawMode;
    }

    public bool IsEraseMode()
    {
        return EraseMode;
    }
}
