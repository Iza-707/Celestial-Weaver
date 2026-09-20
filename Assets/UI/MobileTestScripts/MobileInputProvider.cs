using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

// Reads touch input
// Left half of screen  → move joystick  → MoveInput
// PlayerInputProvider polls the singleton each Update()/OnInput() and merges the result
// with keyboard/mouse. Touch and keyboard can coexist.
//
// Place on a persistent Screen Space Overlay Canvas in the scene (not on the player prefab).
public class MobileInputProvider : MonoBehaviour
{
    public static MobileInputProvider Instance { get; private set; }

    [Header("Joystick Widgets")]
    [SerializeField] private TouchJoystickUI _moveJoystick;

    [Header("Feel")]
    [SerializeField] private float _joystickRadius = 100f; // screen pixels before clamping

    public Vector2 MoveInput { get; private set; }
    private int    _moveTouchId = -1;
    private Vector2 _moveOrigin;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        foreach (var touch in Touch.activeTouches)
        {
            int      id    = touch.touchId;
            Vector2  pos   = touch.screenPosition;
            TouchPhase phase = touch.phase;

            switch (phase)
            {
                case TouchPhase.Began:
                    if (_moveTouchId == -1)
                    {
                        _moveTouchId = id;
                        _moveOrigin  = pos;
                        _moveJoystick.Show(pos);
                    }
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (id == _moveTouchId)
                    {
                        Vector2 delta   = pos - _moveOrigin;
                        Vector2 clamped = Vector2.ClampMagnitude(delta, _joystickRadius);
                        MoveInput = clamped / _joystickRadius;
                        _moveJoystick.SetKnob(clamped);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (id == _moveTouchId)
                    {
                        _moveTouchId = -1;
                        MoveInput    = Vector2.zero;
                        _moveJoystick.Hide();
                    }
                    break;
            }
        }

        // Safety cleanup: if a tracked finger disappeared without an Ended event.
        if (_moveTouchId != -1 && !IsTouchActive(_moveTouchId))
        {
            _moveTouchId = -1;
            MoveInput    = Vector2.zero;
            _moveJoystick.Hide();
        }
    }

    private bool IsTouchActive(int id)
    {
        foreach (var t in Touch.activeTouches)
            if (t.touchId == id) return true;
        return false;
    }
}
