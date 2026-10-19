using UnityEngine;

public class TouchJoystickUI : MonoBehaviour
{
    [SerializeField] private RectTransform _background;
    [SerializeField] private RectTransform _knob;

    private Vector3 _originalKnobPosition;

    private void Awake()
    {
        _originalKnobPosition = _knob.localPosition;
    }

    public void ShowFixed()
    {
        gameObject.SetActive(true);

        ResetKnob();
    }

    public void ResetKnob()
    {
        _knob.localPosition = _originalKnobPosition;
    }

    public Vector2 GetCenter()
    {
        return _background.position;
    }

    public void SetKnob(Vector2 screenDelta)
    {
        _knob.position =
            _background.position +
            new Vector3(
                screenDelta.x,
                screenDelta.y,
                0f
            );
    }
}