using UnityEngine;

/// <summary>
/// AudioManager - Singleton responsible for playing all sound effects in the scene.
///
/// Sound categories:
///   1. Background  — Looping ambient construction site audio, plays automatically on start.
///   2. Footstep    — Movement sounds, played by FootstepAudio.cs while the player moves.
///   3. Interact    — Played when the player clicks on any interactable object.
///   4. TaskDone    — Played when a single task is completed.
///   5. AllDone     — Played when all tasks are completed.
///   6. UIClick     — Played when any UI button is clicked.
///
/// Usage:
///   AudioManager.Instance.PlayInteract();
///   AudioManager.Instance.PlayTaskDone();
///   AudioManager.Instance.PlayAllDone();
///   AudioManager.Instance.PlayUIClick();
///   AudioManager.Instance.PlayFootstep(audioSource);
/// </summary>
public class AudioManager : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Singleton
    // ─────────────────────────────────────────────

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ─────────────────────────────────────────────
    // Inspector Clips
    // ─────────────────────────────────────────────

    [Header("Background")]
    [Tooltip("Looping ambient audio source. Set AudioClip and Loop = true in the Inspector.")]
    public AudioSource backgroundSource;

    [Header("Footstep")]
    [Tooltip("Movement sound clips. One is chosen at random each step.")]
    public AudioClip[] footstepClips;

    [Header("Interaction")]
    [Tooltip("Played when the player clicks on any interactable object.")]
    public AudioClip interactClip;

    [Header("Task")]
    [Tooltip("Played when a single task is completed.")]
    public AudioClip taskDoneClip;

    [Tooltip("Played when all tasks are completed.")]
    public AudioClip allDoneClip;

    [Header("UI")]
    [Tooltip("Played when any UI button is clicked.")]
    public AudioClip uiClickClip;

    // ─────────────────────────────────────────────
    // Volume Settings
    // ─────────────────────────────────────────────

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [Tooltip("Volume for all one-shot sound effects.")]
    public float sfxVolume = 1.0f;

    [Range(0f, 1f)]
    [Tooltip("Volume for the background ambient loop.")]
    public float backgroundVolume = 0.4f;

    // ─────────────────────────────────────────────
    // Internal
    // ─────────────────────────────────────────────

    // Shared AudioSource for all one-shot SFX
    private AudioSource _sfxSource;

    private void Start()
    {
        // Create a shared AudioSource for one-shot SFX at runtime
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;

        // Start background loop automatically on scene load
        StartBackground();
    }

    // ─────────────────────────────────────────────
    // Public API
    // ─────────────────────────────────────────────

    /// <summary>
    /// Plays the interact sound when the player clicks on any interactable object.
    /// </summary>
    public void PlayInteract()
    {
        PlayOneShot(interactClip, "Interact");
    }

    /// <summary>
    /// Plays the task done sound when a single task is completed.
    /// </summary>
    public void PlayTaskDone()
    {
        PlayOneShot(taskDoneClip, "TaskDone");
    }

    /// <summary>
    /// Plays the all done sound when all tasks are completed.
    /// </summary>
    public void PlayAllDone()
    {
        PlayOneShot(allDoneClip, "AllDone");
    }

    /// <summary>
    /// Plays the UI click sound when a button is pressed.
    /// </summary>
    public void PlayUIClick()
    {
        PlayOneShot(uiClickClip, "UIClick");
    }

    /// <summary>
    /// Plays a random footstep clip on the player's AudioSource.
    /// Called by FootstepAudio.cs at regular intervals while moving.
    /// </summary>
    public void PlayFootstep(AudioSource source)
    {
        if (footstepClips == null || footstepClips.Length == 0)
        {
            Debug.LogWarning("[AudioManager] No footstep clips assigned.");
            return;
        }

        // Pick a random clip to avoid repetition
        int index = Random.Range(0, footstepClips.Length);
        AudioClip clip = footstepClips[index];

        if (clip == null) return;

        source.clip = clip;
        source.volume = sfxVolume;
        source.Play();
    }

    /// <summary>
    /// Starts the looping background ambient audio.
    /// Called automatically on Start().
    /// </summary>
    public void StartBackground()
    {
        if (backgroundSource == null)
        {
            Debug.LogWarning("[AudioManager] No backgroundSource assigned.");
            return;
        }

        backgroundSource.volume = backgroundVolume;

        if (!backgroundSource.isPlaying)
            backgroundSource.Play();
    }

    /// <summary>
    /// Stops the background ambient audio.
    /// </summary>
    public void StopBackground()
    {
        if (backgroundSource != null && backgroundSource.isPlaying)
            backgroundSource.Stop();
    }

    // ─────────────────────────────────────────────
    // Internal Helper
    // ─────────────────────────────────────────────

    /// <summary>
    /// Plays a one-shot clip through the shared SFX AudioSource.
    /// </summary>
    private void PlayOneShot(AudioClip clip, string label)
    {
        if (clip == null)
        {
            Debug.LogWarning($"[AudioManager] Clip not assigned: {label}");
            return;
        }

        _sfxSource.PlayOneShot(clip, sfxVolume);
    }
}