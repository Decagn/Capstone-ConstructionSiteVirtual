using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// TaskManager - Singleton that owns the task system at runtime.
///
/// Responsibilities:
///   1. Receive task data from a JSON file in the Resources folder at scene start.
///   2. Expose a method for BuildingElement to report which element was clicked.
///   3. Match the clicked element to the correct task and mark it complete.
///   4. Notify TaskUI to update the display after each state change.
///
/// Setup in Unity Editor:
///   1. Create an empty GameObject named "TaskManager" in the scene.
///   2. Attach this script to it.
///   3. Place your tasks in a lesson JSON file in /Assets/Resources/Lesson Plans/
///   4. The TaskUI component will be found automatically via FindObjectOfType.
///      Alternatively, assign it manually in the Inspector for better performance.
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

    public LessonConfig lessonConfig;
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
            if (!enableRoomFiltering || lessonConfig == null)
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
            if (!enableRoomFiltering || lessonConfig == null)
                return CompletedTaskCount;

            return GetTasksForRoom(_currentRoomId).Count(t => t.isCompleted);
        }
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

    /// <summary>True when all tasks have been completed.</summary>
    public bool AllTasksCompleted => TotalTaskCount > 0 && CompletedTaskCount >= TotalTaskCount;

    /// <summary>
    /// Read-only access to tasks filtered by the current room.
    /// </summary>
    public IReadOnlyList<TaskEntry> CurrentRoomTasks
    {
        get
        {
            if (!enableRoomFiltering || lessonConfig == null)
                return _allTasks;

            return GetTasksForRoom(_currentRoomId);
        }
    }

    /// <summary>Read-only access to the full task list.</summary>
    public IReadOnlyList<TaskEntry> Tasks => _allTasks;

    /// <summary>The lesson title loaded from JSON.</summary>
    public string LessonTitle => lessonConfig?.lessonTitle ?? "Lesson";

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
        // Wait to ensure lessonConfig has been populated by LessonLoader
        yield return new WaitUntil(() => lessonConfig != null);
        LoadLessonConfig();
    }

    // ─────────────────────────────────────────────
    // JSON Loading
    // ─────────────────────────────────────────────

    /// <summary>
    /// Loads the unified lesson configuration.
    /// This includes room definitions and all tasks.
    /// </summary>
    private void LoadLessonConfig()
    {
        if (lessonConfig == null || lessonConfig.rooms == null || lessonConfig.rooms.Count == 0)
        {
            Debug.LogError("[TaskManager] JSON loaded but contains no tasks.");
            return;
        }

        // Build lookup dictionaries
        _taskLookup.Clear();
        _roomLookup.Clear();
        _allTasks.Clear();

        foreach (RoomConfig room in lessonConfig.rooms)
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

        Debug.Log($"[TaskManager] Loaded lesson '{lessonConfig.lessonTitle}' with {lessonConfig.rooms.Count} rooms and {_allTasks.Count} tasks.");

        // Pass room configurations to RoomDetector
        if (roomDetector != null)
        {
            List<RoomData> roomDataList = lessonConfig.rooms.Select(r => r.ToRoomData()).ToList();
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
    /// Plays interact sound regardless of task state.
    /// Marks the task complete if it exists and hasn't been completed yet.
    /// </summary>
    public void OnElementClicked(string elementId, string elementName)
    {
        // Always play interact sound when any interactable object is clicked
        AudioManager.Instance?.PlayInteract();

        if (!_taskLookup.TryGetValue(elementId, out TaskEntry task))
        {
            Debug.Log($"[TaskManager] '{elementName}' clicked — no matching task.");
            return;
        }

        // Task already completed — show reminder feedback
        if (task.isCompleted)
        {
            Debug.Log($"[TaskManager] Task '{task.title}' already completed.");
            taskUI?.ShowFeedback(task, alreadyCompleted: true);
            return;
        }

        // Mark the task as complete
        task.isCompleted = true;
        Debug.Log($"[TaskManager] Task completed: '{task.title}' ({CompletedTaskCount}/{TotalTaskCount})");

        // Play task done sound
        AudioManager.Instance?.PlayTaskDone();

        // Update task panel and show feedback
        taskUI?.RefreshTaskPanel();
        taskUI?.ShowFeedback(task, alreadyCompleted: false);

        // If all tasks are now done, show the results screen
        if (AllTasksCompleted)
            taskUI?.ShowResultsScreen();
    }
}