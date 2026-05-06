using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// FootstepAudio - Plays movement sound effects while the player is moving.
///
/// Because FirstPersonController.cs reads input directly from Keyboard.current
/// (rather than using a PlayerInput component), this script does the same to stay
/// consistent. It checks WASD + E/Q keys each frame and plays a sound at a
/// regular interval whenever any movement key is held down.
///
/// Since the player uses free-fly mode (no gravity/ground contact), the sound
/// functions as a general "movement audio cue" rather than a literal footstep.
/// You can assign any appropriate movement sound in AudioManager's Inspector
/// (e.g. a soft whoosh, a light mechanical hum, or keep footsteps for grounded sections).
///
/// Setup in Unity Editor:
///   1. Attach this script to your Player GameObject (same as FirstPersonController).
///   2. Assign the playerAudioSource field (an AudioSource on the Player).
///   3. Assign footstep clips in AudioManager's Inspector.
///   4. Tune stepInterval to control how frequently the sound plays.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class FootstepAudio : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Inspector Settings
    // ─────────────────────────────────────────────

    [Header("Movement Sound Timing")]
    [Tooltip("Time in seconds between each movement sound while a movement key is held. " +
             "Lower = faster cadence. Default 0.45s roughly matches a walking pace.")]
    [Range(0.1f, 1.0f)]
    public float stepInterval = 0.2f;

    [Header("Audio Source")]
    [Tooltip("The AudioSource on the Player used to emit movement sounds. " +
             "If left empty, the AudioSource on this GameObject is used automatically.")]
    public AudioSource playerAudioSource;

    // ─────────────────────────────────────────────
    // Internal State
    // ─────────────────────────────────────────────

    // Countdown timer. When it reaches zero, a movement sound is played and it resets.
    private float _stepTimer = 0f;

    // Tracks whether the player was moving last frame.
    // Used to reset the timer immediately when movement starts,
    // so the first step plays right away instead of waiting a full interval.
    private bool _wasMoving = false;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Awake()
    {
        // If no AudioSource was assigned in the Inspector, fall back to the one
        // added automatically by [RequireComponent(typeof(AudioSource))].
        if (playerAudioSource == null)
            playerAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        HandleMovementAudio();
    }

    // ─────────────────────────────────────────────
    // Movement Audio Logic
    // ─────────────────────────────────────────────

    /// <summary>
    /// Checks each frame whether any movement key is currently held (WASD or E/Q),
    /// then fires a movement sound at the configured interval.
    ///
    /// This mirrors how FirstPersonController reads input — using Keyboard.current
    /// directly rather than the Input System's action/binding layer.
    /// </summary>
    private void HandleMovementAudio()
    {
        // Safety check: if the keyboard device is not available, do nothing.
        // This can happen in WebGL if the browser hasn't received focus yet.
        if (Keyboard.current == null) return;

        // Check all movement keys that FirstPersonController responds to.
        // WASD = horizontal movement, E/Q = vertical fly movement.
        bool isMoving = Keyboard.current.wKey.isPressed
                     || Keyboard.current.sKey.isPressed
                     || Keyboard.current.aKey.isPressed
                     || Keyboard.current.dKey.isPressed
                     || Keyboard.current.eKey.isPressed
                     || Keyboard.current.qKey.isPressed;

        if (isMoving)
        {
            // If the player just started moving, set the timer to 0 so the first
            // movement sound fires immediately on the same frame, with no delay.
            if (!_wasMoving)
                _stepTimer = 0f;

            // Count down the timer each frame.
            _stepTimer -= Time.deltaTime;

            if (_stepTimer <= 0f)
            {
                // Only play a new sound if the previous clip has finished.
                // This prevents overlapping audio when stepInterval is shorter
                // than the actual duration of the footstep clip.
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayFootstep(playerAudioSource);
                else
                    Debug.LogWarning("[FootstepAudio] AudioManager instance not found in scene.");

                // Always reset the timer to keep the cadence consistent,
                // even if the sound was skipped due to isPlaying being true.
                _stepTimer = stepInterval;
            }
        }
        else
        {
            // No movement key is held — reset the timer so the next movement
            // triggers a sound after one full interval rather than immediately.
            _stepTimer = 0f;
        }

        // Remember this frame's movement state for use next frame.
        _wasMoving = isMoving;
    }
}
