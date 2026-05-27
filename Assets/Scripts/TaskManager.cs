using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// TaskManager - Singleton that owns the task system at runtime.
///
/// NEW: Now loads room and task configuration from a unified JSON file.
/// The JSON file contains both room coordinates and their associated tasks.
///
/// Responsibilities:
///   1. Load lesson configuration from JSON (rooms + tasks).
///   2. Pass room data to RoomDetector for automatic room detection.
///   3. Expose methods for BuildingElement to report clicks.
///   4. Filter tasks by current room.
///   5. Notify TaskUI to update display after state changes.
///
/// Setup in Unity Editor:
///   1. Create an empty GameObject named "TaskManager" in the scene.
///   2. Attach this script to it.
///   3. Place lesson_config.json at: Assets/Resources/lesson_config.json
///   4. The RoomDetector and TaskUI components will be found automatically.
/// </summary>
public class TaskManager : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Singleton
    // ─────────────────────────────────────────────

    public static TaskManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // ─────────────────────────────────────────────
    // Inspector Settings
    // ─────────────────────────────────────────────

    [Header("Lesson Configuration")]
    [Tooltip("Name of the JSON config file inside Assets/Resources/ (without extension).")]
    public string configFileName = "lesson_config";

    [Header("References")]
    [Tooltip("Reference to the TaskUI component. If empty, found automatically at Start().")]
    public TaskUI taskUI;

    [Tooltip("Reference to the RoomDetector component. If empty, found automatically at Start().")]
    public RoomDetector roomDetector;

    [Header("Room Settings")]
    [Tooltip("If true, only tasks matching the current room are shown. If false, all tasks are shown.")]
    public bool enableRoomFiltering = true;

    // ─────────────────────────────────────────────
    // Runtime State
    // ─────────────────────────────────────────────

    private LessonConfig _lessonConfig;
    private Dictionary<string, TaskEntry> _taskLookup = new Dictionary<string, TaskEntry>();
    private Dictionary<string, RoomConfig> _roomLookup = new Dictionary<string, RoomConfig>();

    // The current room ID the player is in (set by RoomDetector)
    private string _currentRoomId = null;

    // Flattened list of all tasks across all rooms (for compatibility)
    private List<TaskEntry> _allTasks = new List<TaskEntry>();

    // ─────────────────────────────────────────────
    // Properties
    // ─────────────────────────────────────────────

    /// <summary>Total number of tasks across all rooms.</summary>
    public int TotalTaskCount => _allTasks?.Count ?? 0;

    /// <summary>Number of tasks completed across all rooms.</summary>
    public int CompletedTaskCount => _allTasks?.Count(t => t.isCompleted) ?? 0;

    /// <summary>
    /// Total number of tasks in the current room only.
    /// </summary>
    public int CurrentRoomTaskCount
    {
        get
        {
            if (!enableRoomFiltering || _lessonConfig == null)
                return TotalTaskCount;

            return GetTasksForRoom(_currentRoomId).Count;
        }
    }

    /// <summary>
    /// Number of completed tasks in the current room only.
    /// </summary>
    public int CurrentRoomCompletedCount
    {
        get
        {
            if (!enableRoomFiltering || _lessonConfig == null)
                return CompletedTaskCount;

            return GetTasksForRoom(_currentRoomId).Count(t => t.isCompleted);
        }
    }

    /// <summary>True when all tasks have been completed.</summary>
    public bool AllTasksCompleted => TotalTaskCount > 0 && CompletedTaskCount >= TotalTaskCount;

    /// <summary>
    /// Read-only access to tasks filtered by the current room.
    /// </summary>
    public IReadOnlyList<TaskEntry> CurrentRoomTasks
    {
        get
        {
            if (!enableRoomFiltering || _lessonConfig == null)
                return _allTasks;

            return GetTasksForRoom(_currentRoomId);
        }
    }

    /// <summary>Read-only access to the full task list.</summary>
    public IReadOnlyList<TaskEntry> Tasks => _allTasks;

    /// <summary>The lesson title loaded from JSON.</summary>
    public string LessonTitle => _lessonConfig?.lessonTitle ?? "Lesson";

    /// <summary>The current room ID the player is in.</summary>
    public string CurrentRoomId => _currentRoomId;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Start()
    {
        // Find TaskUI if not assigned
        if (taskUI == null)
            taskUI = FindFirstObjectByType<TaskUI>();

        if (taskUI == null)
            Debug.LogWarning("[TaskManager] No TaskUI found in scene.");

        // Find RoomDetector if not assigned
        if (roomDetector == null)
            roomDetector = FindFirstObjectByType<RoomDetector>();

        if (roomDetector == null)
            Debug.LogWarning("[TaskManager] No RoomDetector found in scene.");

        StartCoroutine(LoadLessonConfigCoroutine());
    }

    private IEnumerator LoadLessonConfigCoroutine()
    {
        // Wait one frame to ensure all Start() methods have completed
        yield return null;
        LoadLessonConfigFromJSON();
    }

    // ─────────────────────────────────────────────
    // JSON Loading
    // ─────────────────────────────────────────────

    /// <summary>
    /// Loads the unified lesson configuration from JSON.
    /// This includes room definitions and all tasks.
    /// </summary>
    private void LoadLessonConfigFromJSON()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(configFileName);

        if (jsonFile == null)
        {
            Debug.LogError($"[TaskManager] Could not find '{configFileName}.json' in Assets/Resources/.");
            return;
        }

        _lessonConfig = JsonUtility.FromJson<LessonConfig>(jsonFile.text);

        if (_lessonConfig == null || _lessonConfig.rooms == null || _lessonConfig.rooms.Count == 0)
        {
            Debug.LogError("[TaskManager] JSON loaded but contains no rooms.");
            return;
        }

        // Build lookup dictionaries
        _taskLookup.Clear();
        _roomLookup.Clear();
        _allTasks.Clear();

        foreach (RoomConfig room in _lessonConfig.rooms)
        {
            _roomLookup[room.roomId] = room;

            if (room.tasks != null)
            {
                foreach (TaskEntry task in room.tasks)
                {
                    _allTasks.Add(task);

                    if (!string.IsNullOrEmpty(task.targetElementId))
                        _taskLookup[task.targetElementId] = task;
                    else
                        Debug.LogWarning($"[TaskManager] Task '{task.id}' in room '{room.roomId}' has no targetElementId.");
                }
            }
        }

        Debug.Log($"[TaskManager] Loaded lesson '{_lessonConfig.lessonTitle}' with {_lessonConfig.rooms.Count} rooms and {_allTasks.Count} tasks.");

        // Pass room configurations to RoomDetector
        if (roomDetector != null)
        {
            List<RoomData> roomDataList = _lessonConfig.rooms.Select(r => r.ToRoomData()).ToList();
            roomDetector.LoadRoomsFromConfig(roomDataList);
        }

        // Refresh UI
        taskUI?.RefreshTaskPanel();
    }

    // ─────────────────────────────────────────────
    // Public API
    // ─────────────────────────────────────────────

    /// <summary>
    /// Called by RoomDetector when the player enters a new room.
    /// Updates the current room and refreshes the task UI.
    /// </summary>
    public void OnRoomChanged(string newRoomId)
    {
        string previousRoom = _currentRoomId;
        _currentRoomId = newRoomId;

        Debug.Log($"[TaskManager] Room changed from '{previousRoom}' to '{newRoomId}'");

        // Refresh task panel to show tasks for the new room
        taskUI?.RefreshTaskPanel();
    }

    /// <summary>
    /// Called by BuildingElement.OnClick() when the player clicks an interactable object.
    /// </summary>
    public void OnElementClicked(string elementId, string elementName)
    {
        // Play interact sound
        AudioManager.Instance?.PlayInteract();

        if (!_taskLookup.TryGetValue(elementId, out TaskEntry task))
        {
            Debug.Log($"[TaskManager] '{elementName}' clicked — no matching task.");
            return;
        }

        // Task already completed
        if (task.isCompleted)
        {
            Debug.Log($"[TaskManager] Task '{task.title}' already completed.");
            taskUI?.ShowFeedback(task, alreadyCompleted: true);
            return;
        }

        // Mark task as complete
        task.isCompleted = true;
        Debug.Log($"[TaskManager] Task completed: '{task.title}' ({CompletedTaskCount}/{TotalTaskCount})");

        // Play task done sound
        AudioManager.Instance?.PlayTaskDone();

        // Update UI
        taskUI?.RefreshTaskPanel();
        taskUI?.ShowFeedback(task, alreadyCompleted: false);

        // Show results if all tasks done
        if (AllTasksCompleted)
            taskUI?.ShowResultsScreen();
    }

    /// <summary>
    /// Get all tasks for a specific room.
    /// </summary>
    public List<TaskEntry> GetTasksForRoom(string roomId)
    {
        if (string.IsNullOrEmpty(roomId) || !_roomLookup.ContainsKey(roomId))
            return new List<TaskEntry>();

        return _roomLookup[roomId].tasks ?? new List<TaskEntry>();
    }

    /// <summary>
    /// Get the number of completed tasks in a specific room.
    /// </summary>
    public int GetCompletedTasksInRoom(string roomId)
    {
        return GetTasksForRoom(roomId).Count(t => t.isCompleted);
    }

    /// <summary>
    /// Get the total number of tasks in a specific room.
    /// </summary>
    public int GetTotalTasksInRoom(string roomId)
    {
        return GetTasksForRoom(roomId).Count;
    }
}
