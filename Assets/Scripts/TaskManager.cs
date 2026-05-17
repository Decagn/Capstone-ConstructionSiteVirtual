using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// TaskManager - Singleton that owns the task system at runtime.
///
/// Responsibilities:
///   1. Load task data from a JSON file in the Resources folder at scene start.
///   2. Expose a method for BuildingElement to report which element was clicked.
///   3. Match the clicked element to the correct task and mark it complete.
///   4. Filter tasks by room - only show tasks for the current room.
///   5. Notify TaskUI to update the display after each state change or room change.
///
/// Setup in Unity Editor:
///   1. Create an empty GameObject named "TaskManager" in the scene.
///   2. Attach this script to it.
///   3. Place your tasks.json file at: Assets/Resources/tasks.json
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

    [Header("Task Data")]
    [Tooltip("Name of the JSON file inside Assets/Resources/ (without extension).")]
    public string taskFileName = "tasks";

    [Header("References")]
    [Tooltip("Reference to the TaskUI component. If empty, found automatically at Start().")]
    public TaskUI taskUI;

    [Header("Room Settings")]
    [Tooltip("If true, only tasks matching the current room are shown. If false, all tasks are shown.")]
    public bool enableRoomFiltering = true;

    // ─────────────────────────────────────────────
    // Runtime State
    // ─────────────────────────────────────────────

    private TaskList _taskList;
    private Dictionary<string, TaskEntry> _taskLookup = new Dictionary<string, TaskEntry>();
    
    // The current room ID the player is in (set by RoomDetector)
    private string _currentRoomId = null;

    // ─────────────────────────────────────────────
    // Properties
    // ─────────────────────────────────────────────

    /// <summary>Total number of tasks in the current lesson.</summary>
    public int TotalTaskCount => _taskList?.tasks?.Count ?? 0;

    /// <summary>Number of tasks the student has completed so far.</summary>
    public int CompletedTaskCount => _taskList?.tasks?.Count(t => t.isCompleted) ?? 0;

    /// <summary>
    /// Total number of tasks in the current room only.
    /// Used for "X / Y Tasks Completed" display when room filtering is enabled.
    /// </summary>
    public int CurrentRoomTaskCount
    {
        get
        {
            if (!enableRoomFiltering || _taskList == null)
                return TotalTaskCount;
            
            return _taskList.tasks.Count(t => IsTaskInCurrentRoom(t));
        }
    }

    /// <summary>
    /// Number of completed tasks in the current room only.
    /// Used for "X / Y Tasks Completed" display when room filtering is enabled.
    /// </summary>
    public int CurrentRoomCompletedCount
    {
        get
        {
            if (!enableRoomFiltering || _taskList == null)
                return CompletedTaskCount;
            
            return _taskList.tasks.Count(t => t.isCompleted && IsTaskInCurrentRoom(t));
        }
    }

    /// <summary>True when all tasks have been completed.</summary>
    public bool AllTasksCompleted => TotalTaskCount > 0 && CompletedTaskCount >= TotalTaskCount;

    /// <summary>
    /// Read-only access to tasks filtered by the current room.
    /// Returns only tasks that belong to the current room (or tasks with no room specified).
    /// </summary>
    public IReadOnlyList<TaskEntry> CurrentRoomTasks
    {
        get
        {
            if (!enableRoomFiltering || _taskList == null)
                return _taskList?.tasks;

            return _taskList.tasks.Where(t => IsTaskInCurrentRoom(t)).ToList();
        }
    }

    /// <summary>Read-only access to the full task list for UI rendering.</summary>
    public IReadOnlyList<TaskEntry> Tasks => _taskList?.tasks;

    /// <summary>The lesson title loaded from JSON.</summary>
    public string LessonTitle => _taskList?.lessonTitle ?? "Lesson";

    /// <summary>The current room ID the player is in.</summary>
    public string CurrentRoomId => _currentRoomId;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Start()
    {
        if (taskUI == null)
            taskUI = FindFirstObjectByType<TaskUI>();

        if (taskUI == null)
            Debug.LogWarning("[TaskManager] No TaskUI found in scene.");

        StartCoroutine(LoadTasksCoroutine());
    }

    private IEnumerator LoadTasksCoroutine()
    {
        // Wait one frame to ensure all Start() methods have completed
        yield return null;
        LoadTasksFromJSON();
    }

    // ─────────────────────────────────────────────
    // JSON Loading
    // ─────────────────────────────────────────────

    private void LoadTasksFromJSON()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(taskFileName);

        if (jsonFile == null)
        {
            Debug.LogError($"[TaskManager] Could not find '{taskFileName}.json' in Assets/Resources/.");
            return;
        }

        _taskList = JsonUtility.FromJson<TaskList>(jsonFile.text);

        if (_taskList == null || _taskList.tasks == null || _taskList.tasks.Count == 0)
        {
            Debug.LogError("[TaskManager] JSON loaded but contains no tasks.");
            return;
        }

        _taskLookup.Clear();
        foreach (TaskEntry task in _taskList.tasks)
        {
            if (!string.IsNullOrEmpty(task.targetElementId))
                _taskLookup[task.targetElementId] = task;
            else
                Debug.LogWarning($"[TaskManager] Task '{task.id}' has no targetElementId.");
        }

        Debug.Log($"[TaskManager] Loaded lesson '{_taskList.lessonTitle}' with {_taskList.tasks.Count} tasks.");
        taskUI?.RefreshTaskPanel();
    }

    // ─────────────────────────────────────────────
    // Room Filtering Logic
    // ─────────────────────────────────────────────

    /// <summary>
    /// Checks if a task belongs to the current room.
    /// A task belongs to the current room if:
    ///   - Its roomId matches the current room ID, OR
    ///   - Its roomId is empty/null (global task, shown in all rooms), OR
    ///   - Its roomId is "Any" (shown in all rooms)
    /// </summary>
    private bool IsTaskInCurrentRoom(TaskEntry task)
    {
        // If room filtering is disabled, all tasks are shown
        if (!enableRoomFiltering)
            return true;

        // Tasks with no room ID are shown in all rooms
        if (string.IsNullOrEmpty(task.roomId) || task.roomId == "Any")
            return true;

        // Task belongs to the current room
        return task.roomId == _currentRoomId;
    }

    // ─────────────────────────────────────────────
    // Public API
    // ─────────────────────────────────────────────

    /// <summary>
    /// Called by RoomDetector when the player enters a new room.
    /// Updates the current room and refreshes the task UI to show only tasks for this room.
    /// </summary>
    /// <param name="newRoomId">The room ID the player just entered</param>
    public void OnRoomChanged(string newRoomId)
    {
        string previousRoom = _currentRoomId;
        _currentRoomId = newRoomId;

        Debug.Log($"[TaskManager] Room changed from '{previousRoom}' to '{newRoomId}'");

        // Refresh task panel to show tasks for the new room
        taskUI?.RefreshTaskPanel();

        // Optionally: show a notification that the room changed
        // taskUI?.ShowRoomChangeNotification(newRoomId);
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

    /// <summary>
    /// Get all tasks for a specific room.
    /// Useful for showing a preview of what tasks are in each room.
    /// </summary>
    public List<TaskEntry> GetTasksForRoom(string roomId)
    {
        if (_taskList == null)
            return new List<TaskEntry>();

        return _taskList.tasks.Where(t => 
            t.roomId == roomId || 
            string.IsNullOrEmpty(t.roomId) || 
            t.roomId == "Any"
        ).ToList();
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
