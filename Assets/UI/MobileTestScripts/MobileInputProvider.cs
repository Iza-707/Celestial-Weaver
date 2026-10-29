using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class MobileInputProvider : MonoBehaviour
{
    public static MobileInputProvider Instance { get; private set; }

    [Header("Joystick")]
    [SerializeField] private TouchJoystickUI _moveJoystick;

    [SerializeField] private float _joystickRadius = 100f;

    [Header("Joystick Touch Area")]
    [SerializeField] private RectTransform _moveTouchArea;
    [SerializeField, Range(0.1f, 0.5f)] private float _touchAreaWidth = 0.28f;
    [SerializeField, Range(0.1f, 0.75f)] private float _touchAreaHeight = 0.38f;

    public Vector2 MoveInput { get; private set; }

    private int _moveTouchId = -1;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();

        if (_moveTouchArea != null)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
                _moveTouchArea.SetParent(canvas.transform, false);

            _moveTouchArea.anchorMin = Vector2.zero;
            _moveTouchArea.anchorMax = new Vector2(_touchAreaWidth, _touchAreaHeight);
            _moveTouchArea.pivot = Vector2.zero;
            _moveTouchArea.offsetMin = Vector2.zero;
            _moveTouchArea.offsetMax = Vector2.zero;
            _moveTouchArea.anchoredPosition = Vector2.zero;

            Graphic touchAreaGraphic = _moveTouchArea.GetComponent<Graphic>();
            if (touchAreaGraphic != null)
                touchAreaGraphic.raycastTarget = false;

            if (canvas != null && _moveJoystick != null)
                _moveJoystick.transform.SetParent(canvas.transform, false);
        }

        if (_moveJoystick != null)
            _moveJoystick.ShowFixed();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        _moveTouchId = -1;
        MoveInput = Vector2.zero;

        if (_moveJoystick != null)
            _moveJoystick.EndTouch();
    }

    public bool IsTouchOverJoystick(Vector2 screenPosition)
    {
        return _moveJoystick != null &&
            _moveJoystick.ContainsScreenPoint(screenPosition);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        foreach (var touch in Touch.activeTouches)
        {
            int id = touch.touchId;
            Vector2 pos = touch.screenPosition;
            TouchPhase phase = touch.phase;

            switch (phase)
            {
                case TouchPhase.Began:

                    if (_moveTouchId == -1 &&
                        _moveTouchArea != null &&
                        RectTransformUtility.RectangleContainsScreenPoint(
                            _moveTouchArea,
                            pos,
                            null
                            )
                        )
                    {
                        _moveTouchId = id;
                    }

                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:

                    if (id == _moveTouchId)
                    {
                        if (_moveJoystick == null)
                            break;

                        Vector2 delta =
                            pos - _moveJoystick.GetCenter();

                        Vector2 clamped =
                            Vector2.ClampMagnitude(
                                delta,
                                _joystickRadius
                            );

                        MoveInput =
                            clamped / _joystickRadius;

                        _moveJoystick.SetKnob(clamped);
                    }

                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:

                    if (id == _moveTouchId)
                    {
                        _moveTouchId = -1;
                        MoveInput = Vector2.zero;

                        _moveJoystick.EndTouch();
                    }

                    break;
            }
        }

        if (_moveTouchId != -1 &&
            !IsTouchActive(_moveTouchId))
        {
            _moveTouchId = -1;
            MoveInput = Vector2.zero;

            _moveJoystick.EndTouch();
        }
    }

    private bool IsTouchActive(int id)
    {
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.touchId == id)
                return true;
        }

        return false;
    }
}