using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

// Editor-only helper: turns mouse clicks into simulated touches so you can test
// touch UI without a device. Toggle with the checkbox or the hotkey during Play mode.
// Does nothing in builds.
public class TouchSimulationToggle : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private bool _simulateTouch = true;
    [SerializeField] private Key _toggleKey = Key.T;

    private void OnEnable()  => Apply();
    private void OnDisable() => SetSimulation(false);

    // Fires when you tick/untick the checkbox in the Inspector during Play mode.
    private void OnValidate()
    {
        if (Application.isPlaying && isActiveAndEnabled) Apply();
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard[_toggleKey].wasPressedThisFrame)
        {
            _simulateTouch = !_simulateTouch;
            Apply();
            Debug.Log($"Touch simulation: {(_simulateTouch ? "ON" : "OFF")}");
        }
    }

    private void Apply() => SetSimulation(_simulateTouch);

    private static void SetSimulation(bool on)
    {
        if (on) TouchSimulation.Enable();
        else    TouchSimulation.Disable();
    }
#endif
}