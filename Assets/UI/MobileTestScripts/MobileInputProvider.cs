using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
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

    public Vector2 MoveInput { get; private set; }

    private int _moveTouchId = -1;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();

        // Show the joystick at its fixed UI position.
        if (_moveJoystick != null)
            _moveJoystick.ShowFixed();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
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
                        Vector2 joystickCenter =
                            _moveJoystick.GetCenter();

                        Vector2 delta =
                            pos - joystickCenter;

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

                        _moveJoystick.ResetKnob();
                    }

                    break;
            }
        }

        if (_moveTouchId != -1 &&
            !IsTouchActive(_moveTouchId))
        {
            _moveTouchId = -1;
            MoveInput = Vector2.zero;

            _moveJoystick.ResetKnob();
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