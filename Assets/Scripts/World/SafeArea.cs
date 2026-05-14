using UnityEngine;

/// <summary>
/// SafeArea - Adjusts the RectTransform of this panel to fit within the device's safe area.
///
/// Safe area accounts for notches, rounded corners, and home indicators on modern phones.
/// Attach this script to a Panel that is a direct child of the Canvas.
/// All UI elements should be placed inside this Panel instead of directly on the Canvas.
///
/// Usage:
///   1. Create a Panel under your Canvas, name it "SafeAreaPanel"
///   2. Attach this script to SafeAreaPanel
///   3. Move all UI elements inside SafeAreaPanel
/// </summary>
public class SafeArea : MonoBehaviour
{
    // The RectTransform of this panel — will be resized to match the safe area
    private RectTransform _rectTransform;

    // The Canvas this panel belongs to — needed to convert safe area to local coordinates
    private Canvas _canvas;

    // The last safe area we applied — used to detect changes (e.g. device rotation)
    private Rect _lastSafeArea = Rect.zero;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

        // Apply safe area immediately on startup
        ApplySafeArea();
    }

    private void Update()
    {
        // Re-apply if the safe area changes (e.g. device rotated)
        if (_lastSafeArea != Screen.safeArea)
            ApplySafeArea();
    }

    /// <summary>
    /// Adjusts the panel's anchors to match the device's safe area.
    /// Converts the safe area from screen pixels to normalized anchor values (0-1).
    /// </summary>
    private void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;

        // Convert safe area from pixel coordinates to normalized anchor coordinates
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        // Divide by screen dimensions to get values between 0 and 1
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // Apply to RectTransform
        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
        _rectTransform.offsetMin = Vector2.zero;
        _rectTransform.offsetMax = Vector2.zero;

        // Cache the applied safe area to detect future changes
        _lastSafeArea = safeArea;
    }
}