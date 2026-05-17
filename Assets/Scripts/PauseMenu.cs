using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// PauseMenu - Manages the pause state of the game.
///
/// Features:
///   - Press P on desktop to toggle the pause menu
///   - Mobile users tap the PauseButton which calls TogglePause()
///   - Resume restores game time and cursor lock
///   - Restart reloads the current scene from scratch
///   - Quit exits the application (mobile only — hidden on desktop automatically)
///
/// Setup in Unity Editor:
///   1. Attach this script to PauseMenuCanvas
///   2. Assign PausePanel and QuitButton in the Inspector
///   3. Bind ResumeButton OnClick -> PauseMenu.Resume()
///   4. Bind RestartButton OnClick -> PauseMenu.Restart()
///   5. Bind QuitButton OnClick -> PauseMenu.Quit()
///   6. Set PauseMenuCanvas Sort Order to 10 so it renders above all other UI
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The Panel GameObject containing all pause menu buttons. Inactive by default.")]
    public GameObject pausePanel;

    [Tooltip("The Quit button GameObject — shown on mobile only, hidden on desktop.")]
    public GameObject quitButton;

    // Tracks whether the game is currently paused
    private bool _isPaused = false;

    /// <summary>
    /// Called once before the first frame.
    /// Ensures the pause panel is hidden at scene start.
    /// Hides the Quit button on desktop — on WebGL, closing the browser tab is the equivalent.
    /// </summary>
    private void Start()
    {
        // Hide pause panel at scene start
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Show Quit button on mobile only
        if (quitButton != null)
        {
#if UNITY_EDITOR
            // Always hide Quit button in the Editor for desktop testing
            quitButton.SetActive(false);
#elif UNITY_ANDROID || UNITY_IOS
            // Always show on native mobile builds
            quitButton.SetActive(true);
#elif UNITY_WEBGL
            // In WebGL, show on mobile devices, hide on desktop browsers
            bool isMobile = Input.touchSupported && !Input.mousePresent;
            quitButton.SetActive(isMobile);
#else
            quitButton.SetActive(false);
#endif
        }
    }

    /// <summary>
    /// Called once per frame.
    /// Listens for the P key on desktop to toggle the pause menu.
    /// Mobile users use the PauseButton which calls TogglePause() directly.
    /// </summary>
    private void Update()
    {
        // Check keyboard is available (won't be on mobile)
        if (Keyboard.current == null) return;

        // Press P to toggle pause — ESC is avoided in WebGL as it exits fullscreen
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (_isPaused)
                Resume();
            else
                Pause();
        }
    }

    /// <summary>
    /// Called by the mobile PauseButton to toggle the pause menu.
    /// Switches between paused and resumed state.
    /// </summary>
    public void TogglePause()
    {
        if (_isPaused)
            Resume();
        else
            Pause();
    }

    /// <summary>
    /// Pauses the game by freezing time and showing the pause panel.
    /// Unlocks the cursor so the player can click the menu buttons.
    /// </summary>
    public void Pause()
    {
        // Show the pause panel
        if (pausePanel != null)
            pausePanel.SetActive(true);

        // Freeze all game time — stops physics, animations, and most Update calls
        Time.timeScale = 0f;

        // Unlock cursor so player can interact with buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _isPaused = true;

        Debug.Log("[PauseMenu] Game paused.");
    }

    /// <summary>
    /// Resumes the game by restoring time and hiding the pause panel.
    /// </summary>
    public void Resume()
    {
        // Hide the pause panel
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Restore normal game time
        Time.timeScale = 1f;

        _isPaused = false;

        Debug.Log("[PauseMenu] Game resumed.");
    }

    /// <summary>
    /// Restarts the current scene by reloading it from scratch.
    /// Restores time scale before reloading to prevent the new scene starting frozen.
    /// </summary>
    public void Restart()
    {
        // Must restore time scale before reload — otherwise new scene starts frozen
        Time.timeScale = 1f;

        // Reload the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        Debug.Log("[PauseMenu] Scene restarted.");
    }

    /// <summary>
    /// Quits the application. Only shown and functional on mobile.
    /// On desktop WebGL, the player closes the browser tab instead.
    /// Has no effect in the Unity Editor.
    /// </summary>
    public void Quit()
    {
        Time.timeScale = 1f;
        Application.Quit();

        // This log confirms Quit was called during Editor testing
        Debug.Log("[PauseMenu] Quit called. Only exits in a built application.");
    }
}