using UnityEngine;

public class TouchJoystickUI : MonoBehaviour
{
    [SerializeField] private RectTransform _background;
    [SerializeField] private RectTransform _knob;
    [SerializeField] private float _edgePadding = 12f;

    public void ShowFixed()
    {
        gameObject.SetActive(true);

        RectTransform joystickRect = transform as RectTransform;
        if (joystickRect != null)
        {
            joystickRect.anchorMin = Vector2.zero;
            joystickRect.anchorMax = Vector2.zero;
            joystickRect.pivot = new Vector2(0.5f, 0.5f);
            joystickRect.anchoredPosition = new Vector2(
                _edgePadding + joystickRect.sizeDelta.x * Mathf.Abs(joystickRect.localScale.x) * 0.5f,
                _edgePadding + joystickRect.sizeDelta.y * Mathf.Abs(joystickRect.localScale.y) * 0.5f
            );
        }

        ResetKnob();
    }

    public void EndTouch()
    {
        ResetKnob();
    }

    public void ResetKnob()
    {
        _knob.position = _background.position;
    }

    public Vector2 GetCenter()
    {
        return RectTransformUtility.WorldToScreenPoint(
            GetCanvasCamera(),
            _background.position
        );
    }

    public void SetKnob(Vector2 screenDelta)
    {
        Vector2 knobScreenPosition = GetCenter() + screenDelta;
        RectTransform knobParent = _knob.parent as RectTransform;
        if (knobParent != null &&
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                knobParent,
                knobScreenPosition,
                GetCanvasCamera(),
                out Vector3 worldPosition))
        {
            _knob.position = worldPosition;
        }
    }

    public bool ContainsScreenPoint(Vector2 screenPosition)
    {
        Camera camera = GetCanvasCamera();
        return RectTransformUtility.RectangleContainsScreenPoint(
                _background,
                screenPosition,
                camera
            ) ||
            RectTransformUtility.RectangleContainsScreenPoint(
                _knob,
                screenPosition,
                camera
            );
    }

    private Camera GetCanvasCamera()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        return canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera
            : null;
    }
}