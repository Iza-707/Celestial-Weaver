using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

// Editor-only helper: turns mouse clicks into simulated touches.
// Toggle with T during Play mode.
// In an actual mobile build, the mobile UI remains enabled.
public class TouchSimulationToggle : MonoBehaviour
{
#if UNITY_EDITOR

    public static bool IsSimulationEnabled { get; private set; }

    [SerializeField] private bool _simulateTouch = true;
    [SerializeField] private Key _toggleKey = Key.T;

    private void OnEnable()
    {
        Apply();
    }

    private void OnDisable()
    {
        SetSimulation(false);
    }

    private void OnValidate()
    {
        if (Application.isPlaying && isActiveAndEnabled)
            Apply();
    }

    private void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard != null &&
            keyboard[_toggleKey].wasPressedThisFrame)
        {
            _simulateTouch = !_simulateTouch;
            Apply();

            Debug.Log(
                $"Touch simulation: {(_simulateTouch ? "ON" : "OFF")}"
            );
        }
    }

    private void Apply()
    {
        SetSimulation(_simulateTouch);
    }

    private static void SetSimulation(bool on)
    {
        IsSimulationEnabled = on;

        if (on)
            TouchSimulation.Enable();
        else
            TouchSimulation.Disable();
    }

#else

    // Actual mobile builds should always have mobile controls available.
    public static bool IsSimulationEnabled => true;

#endif
}