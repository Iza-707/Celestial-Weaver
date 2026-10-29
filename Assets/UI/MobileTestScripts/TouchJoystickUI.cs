using UnityEngine;

public class TouchJoystickUI : MonoBehaviour
{
    [SerializeField] private RectTransform _background;
    [SerializeField] private RectTransform _knob;
    [SerializeField] private float _edgePadding = 12f;
    [SerializeField] private float _maximumCenterOffset = 48f;

    private Vector2 _homeCenterScreenPosition;
    private bool _hasHomePosition;

    public void InitializeHomePosition()
    {
        gameObject.SetActive(true);

        Rect safeArea = Screen.safeArea;
        Vector3[] corners = new Vector3[4];
        _background.GetWorldCorners(corners);
        Vector2 bottomLeft = RectTransformUtility.WorldToScreenPoint(GetCanvasCamera(), corners[0]);
        Vector2 topRight = RectTransformUtility.WorldToScreenPoint(GetCanvasCamera(), corners[2]);
        Vector2 halfSize = (topRight - bottomLeft) * 0.5f;

        Vector2 homeCenter = new Vector2(
            safeArea.xMin + halfSize.x + _edgePadding,
            safeArea.yMin + halfSize.y + _edgePadding
        );

        SetCenterAtScreenPosition(homeCenter);
        _homeCenterScreenPosition = GetCenter();
        _hasHomePosition = true;

        ResetKnob();
    }

    public void BeginTouch(Vector2 screenPosition, float radius)
    {
        gameObject.SetActive(true);

        Vector2 offsetFromHome = screenPosition - _homeCenterScreenPosition;
        Vector2 centerPosition = _homeCenterScreenPosition +
            Vector2.ClampMagnitude(offsetFromHome, _maximumCenterOffset);

        SetCenterAtScreenPosition(centerPosition);

        Vector2 delta = screenPosition - GetCenter();
        Vector2 clampedDelta = Vector2.ClampMagnitude(delta, radius);
        SetKnobAtScreenPosition(GetCenter() + clampedDelta);
    }

    public void EndTouch()
    {
        ResetKnob();
    }

    public void ResetKnob()
    {
        if (_hasHomePosition)
            SetCenterAtScreenPosition(_homeCenterScreenPosition);

        _knob.position = _background.position;
    }

    public Vector2 GetCenter()
    {
        return RectTransformUtility.WorldToScreenPoint(
            GetCanvasCamera(),
            _background.position
        );
    }

    public void SetCenterAtScreenPosition(Vector2 screenPosition)
    {
        RectTransform joystickRect = transform as RectTransform;
        RectTransform parentRect = joystickRect.parent as RectTransform;
        if (parentRect == null)
            return;

        Vector2 currentCenter = GetCenter();
        Camera camera = GetCanvasCamera();

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                parentRect,
                currentCenter,
                camera,
                out Vector3 currentWorldPosition) &&
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                parentRect,
                screenPosition,
                camera,
                out Vector3 targetWorldPosition))
        {
            joystickRect.position += targetWorldPosition - currentWorldPosition;
        }
    }

    public void SetKnobAtScreenPosition(Vector2 screenPosition)
    {
        RectTransform knobParent = _knob.parent as RectTransform;
        if (knobParent != null &&
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                knobParent,
                screenPosition,
                GetCanvasCamera(),
                out Vector3 worldPosition))
        {
            _knob.position = worldPosition;
        }
    }

    private Camera GetCanvasCamera()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        return canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera
            : null;
    }
}