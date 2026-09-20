using UnityEngine;

// A single joystick widget for touch input.
// The GameObject root is hidden by default and shown only while a finger is down.
// Position is driven externally by MobileInputProvider — this class is purely visual.
//
// Setup: attach to a child GameObject of the mobile overlay Canvas.
// Assign _background (outer ring Image) and _knob (inner circle Image) in the Inspector.
// Both RectTransforms should have their anchors set to top-left (0,0) for clean position math.
public class TouchJoystickUI : MonoBehaviour
{
    [SerializeField] private RectTransform _background;
    [SerializeField] private RectTransform _knob;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    // Show the joystick, anchored at the screen position where the finger landed.
    public void Show(Vector2 screenPos)
    {
        gameObject.SetActive(true);
        _background.position = new Vector3(screenPos.x, screenPos.y, 0f);
        _knob.position = _background.position;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // screenDelta: offset from the origin in screen pixels (already clamped by MobileInputProvider).
    public void SetKnob(Vector2 screenDelta)
    {
        _knob.position = _background.position + new Vector3(screenDelta.x, screenDelta.y, 0f);
    }
}
