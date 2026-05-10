using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// PauseMenu manages the pause state of the game.
/// It handles showing/hiding the pause panel, freezing game time,
/// resuming gameplay, restarting the scene, and quitting the application.
/// 
/// Attach this script to the PauseMenuCanvas GameObject.
/// Assign the PausePanel reference in the Inspector.
/// The player can toggle the pause menu by pressing the Escape key.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    // Reference to the Panel GameObject that contains all pause menu UI elements.
    // This panel is shown when the game is paused and hidden when the game is running.
    public GameObject pausePanel;

    // Tracks whether the game is currently paused.
    // Used to toggle between paused and resumed states.
    private bool _isPaused = false;

    /// <summary>
    /// Called once before the first frame.
    /// Ensures the pause panel is hidden when the game starts.
    /// </summary>
    void Start()
    {
        // Make sure the pause menu is not visible at the start of the game.
        pausePanel.SetActive(false);
    }

    /// <summary>
    /// Called by the mobile pause button to toggle the pause menu.
    /// </summary>
    public void TogglePause()
    {
        if (_isPaused)
            Resume();
        else
            Pause();
    }

    /// <summary>
    /// Called once per frame.
    /// Listens for the Escape key to toggle the pause menu on and off.
    /// </summary>
    void Update()
    {
        // Use the new Input System to detect the Escape key press.
        // Keyboard.current.escapeKey.wasPressedThisFrame ensures the toggle
        // only fires once per key press, not continuously while held down.
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (_isPaused)
                Resume();
            else
                Pause();
        }
    }

    /// <summary>
    /// Pauses the game by freezing time and showing the pause panel.
    /// Also unlocks the cursor so the player can interact with the UI buttons.
    /// </summary>
    public void Pause()
    {
        // Show the pause menu panel.
        pausePanel.SetActive(true);

        // Freeze all game time. This stops physics, animations, and Update calls
        // on most objects, effectively pausing the game world.
        Time.timeScale = 0f;

        // Unlock and show the cursor so the player can click the menu buttons.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _isPaused = true;
    }

    /// <summary>
    /// Resumes the game by restoring time and hiding the pause panel.
    /// Also re-locks the cursor for first-person camera control.
    /// </summary>
    public void Resume()
    {
        // Hide the pause menu panel.
        pausePanel.SetActive(false);

        // Restore normal game time.
        Time.timeScale = 1f;

        // Re-lock the cursor to the center of the screen for first-person look controls.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _isPaused = false;
    }

    /// <summary>
    /// Restarts the current scene by reloading it from scratch.
    /// Also restores time scale in case it was frozen when this is called.
    /// Call this method from the RestartButton's OnClick event in the Inspector.
    /// </summary>
    public void Restart()
    {
        // Restore time scale before reloading, otherwise the new scene
        // will also start with time frozen.
        Time.timeScale = 1f;

        // Reload the currently active scene by its build index.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Quits the application.
    /// In the Unity Editor this has no visible effect, but it works in a built application.
    /// Call this method from the QuitButton's OnClick event in the Inspector.
    /// </summary>
    public void Quit()
    {
        // Restore time scale before quitting for clean shutdown.
        Time.timeScale = 1f;

        // Exit the application. This only works in a built player,
        // not in the Unity Editor.
        Application.Quit();

        // Log a message so we can confirm the Quit function was called during Editor testing.
        Debug.Log("[PauseMenu] Quit called. This only exits in a built application.");
    }
}