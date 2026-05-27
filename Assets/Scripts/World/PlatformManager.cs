using UnityEngine;

/// <summary>
/// PlatformManager - Detects whether the game is running on mobile or desktop
/// and enables/disables the appropriate components and canvases accordingly.
///
/// Attach this script to any persistent GameObject in the scene (e.g. GameManager).
/// Assign all references in the Inspector.
/// </summary>
public class PlatformManager : MonoBehaviour
{
    [Header("Desktop Only")]
    public FirstPersonController firstPersonController;

    [Header("Mobile Only")]
    public MobileInputHandler mobileInputHandler;

    [Tooltip("The Canvas containing mobile UI — joystick, buttons, etc.")]
    public GameObject mobileCanvas;

    [Tooltip("The HUD Canvas containing crosshair and mobile interaction UI.")]
    public GameObject hudCanvas;

    private void Awake()
    {
        // Detect if running on a mobile device.
        // This works correctly in both Editor (with device simulation) and WebGL builds.
        bool isMobile = IsMobilePlatform();

        if (isMobile)
            SetupMobile();
        else
            SetupDesktop();

        //Debug.Log($"[PlatformManager] Running as {(isMobile ? "Mobile" : "Desktop")}");
    }

    /// <summary>
    /// Returns true if the current platform is a mobile device.
    /// Checks for Android, iOS, and WebGL running on a touchscreen device.
    /// </summary>
    private bool IsMobilePlatform()
    {
        //#if UNITY_EDITOR
        //        // Always treat as desktop when running in the Unity Editor
        //        return false;
        //#elif UNITY_ANDROID || UNITY_IOS
        //    return true;
        //#elif UNITY_WEBGL
        //    return Input.touchSupported && !Input.mousePresent;
        //#else
        //    return false;
        //#endif
        return Application.isMobilePlatform || UnityEngine.Device.Application.isMobilePlatform;
    }

    /// <summary>
    /// Configures the scene for desktop mode.
    /// Enables keyboard/mouse input, disables mobile UI and touch controls.
    /// </summary>
    private void SetupDesktop()
    {
        // Enable desktop input controller
        if (firstPersonController != null)
            firstPersonController.enabled = true;

        // Disable mobile input controller
        if (mobileInputHandler != null)
            mobileInputHandler.enabled = false;

        // Hide mobile UI canvas
        if (mobileCanvas != null)
            mobileCanvas.SetActive(false);

        // Hide HUD canvas (no crosshair needed on desktop — using mouse)
        if (hudCanvas != null)
            hudCanvas.SetActive(false);
    }

    /// <summary>
    /// Configures the scene for mobile mode.
    /// Enables touch controls and mobile UI, disables keyboard/mouse input.
    /// </summary>
    private void SetupMobile()
    {
        // Disable desktop input controller
        if (firstPersonController != null)
            firstPersonController.enabled = false;

        // Enable mobile input controller
        if (mobileInputHandler != null)
            mobileInputHandler.enabled = true;

        // Show mobile UI canvas
        if (mobileCanvas != null)
            mobileCanvas.SetActive(true);

        // Show HUD canvas on mobile
        if (hudCanvas != null)
            hudCanvas.SetActive(true);
    }
}