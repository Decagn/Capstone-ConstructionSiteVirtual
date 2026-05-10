using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// FullScreenToggle allows the player to toggle between fullscreen and windowed mode
/// by pressing the F key during gameplay.
/// 
/// Attach this script to any persistent GameObject in the scene (e.g. a Manager object).
/// This script uses Unity's new Input System for key detection.
/// </summary>
public class FullScreenToggle : MonoBehaviour
{
    /// <summary>
    /// Called once per frame.
    /// Checks if the F key was pressed and toggles fullscreen mode accordingly.
    /// </summary>
    void Update()
    {
        // Detect a single press of the F key using the new Input System.
        // wasPressedThisFrame ensures the toggle only fires once per key press,
        // not continuously while the key is held down.
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            // Toggle between fullscreen and windowed mode.
            // Screen.fullScreen returns true if currently fullscreen, false if windowed.
            Screen.fullScreen = !Screen.fullScreen;

            Debug.Log($"[FullScreenToggle] Fullscreen set to: {Screen.fullScreen}");
        }
    }
    /// <summary>
    /// Toggles fullscreen mode when called from a UI button.
    /// Wire this up to a Button's OnClick event in the Inspector.
    /// This is the mobile-friendly equivalent of the F key shortcut.
    /// </summary>
    public void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        Debug.Log($"[FullScreenToggle] Fullscreen set to: {Screen.fullScreen}");
    }
}