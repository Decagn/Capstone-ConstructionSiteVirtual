using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// TaskUI - Manages all task-related UI elements in the scene.
///
/// Controls four UI areas:
///   1. Task Panel    — Scrollable list of tasks with completion checkmarks.
///   2. Progress Bar  — Shows X/TotalTasks completed with a fill bar.
///   3. Feedback Panel — Brief pop-up after clicking a task element.
///   4. Results Screen — Shown when all tasks are complete, auto-hides after delay.
///
/// MODIFICATION: Now supports room-based task filtering.
/// Only displays tasks for the current room when room filtering is enabled.
/// </summary>
public class TaskUI : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Inspector References
    // ─────────────────────────────────────────────

    [Header("Task Panel")]
    [Tooltip("The parent Transform where task row prefabs are instantiated.")]
    public Transform taskListContent;

    [Tooltip("Prefab for a single task row. Must contain a Toggle and a TMP_Text label.")]
    public GameObject taskRowPrefab;

    [Header("Progress Bar")]
    [Tooltip("Image component used as the progress bar fill. Set Image Type to Filled.")]
    public Image progressBarFill;

    [Tooltip("TextMeshPro label showing X / Y Tasks Completed.")]
    public TMP_Text progressText;

    [Header("Feedback Panel")]
    [Tooltip("The root GameObject of the feedback pop-up. Must be inactive by default.")]
    public GameObject feedbackPanel;

    [Tooltip("Title text in the feedback panel — shows the task name.")]
    public TMP_Text feedbackTitle;

    [Tooltip("Body text in the feedback panel — shows the task's feedbackText from JSON.")]
    public TMP_Text feedbackBody;

    [Tooltip("How many seconds the feedback panel stays visible before auto-hiding.")]
    public float feedbackDisplayDuration = 3f;

    [Header("Results Screen")]
    [Tooltip("The root GameObject of the lesson completion screen. Must be inactive by default.")]
    public GameObject resultsPanel;

    [Tooltip("Text displayed on the results screen when all tasks are complete.")]
    public TMP_Text resultsText;

    [Tooltip("How many seconds the results screen stays visible before auto-hiding.")]
    public float resultsDisplayDuration = 5f;

    // ─────────────────────────────────────────────
    // Internal State
    // ─────────────────────────────────────────────

    // Coroutine handle for hiding the feedback panel
    private Coroutine _feedbackCoroutine;

    // All instantiated task row GameObjects
    private List<GameObject> _taskRows = new List<GameObject>();

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Start()
    {
        // Hide both panels at scene start
        if (feedbackPanel != null) feedbackPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(false);
    }

    // ─────────────────────────────────────────────
    // Public API
    // ─────────────────────────────────────────────

    /// <summary>
    /// Rebuilds the task list panel and updates the progress bar.
    /// Called by TaskManager after loading JSON and after each task completion.
    /// MODIFIED: Now uses CurrentRoomTasks instead of all tasks when room filtering is enabled.
    /// </summary>
    public void RefreshTaskPanel()
    {
        if (TaskManager.Instance == null) return;
        RebuildTaskList();
        UpdateProgressBar();
    }

    /// <summary>
    /// Shows the feedback panel after a BuildingElement is clicked.
    /// Auto-hides after feedbackDisplayDuration seconds.
    /// </summary>
    public void ShowFeedback(TaskEntry task, bool alreadyCompleted)
    {
        if (feedbackPanel == null) return;

        // Cancel any running auto-hide timer
        if (_feedbackCoroutine != null)
            StopCoroutine(_feedbackCoroutine);

        if (feedbackTitle != null)
            feedbackTitle.text = alreadyCompleted
                ? $"[Done] {task.title}"
                : $"Task Complete: {task.title}";

        if (feedbackBody != null)
            feedbackBody.text = alreadyCompleted
                ? "You have already completed this task."
                : task.feedbackText;

        feedbackPanel.SetActive(true);
        _feedbackCoroutine = StartCoroutine(HideFeedbackAfterDelay());
    }

    /// <summary>
    /// Shows the lesson completion screen when all tasks are done.
    /// Waits 2 seconds so the feedback panel for the last task has time to display.
    /// Auto-hides after resultsDisplayDuration seconds.
    /// </summary>
    public void ShowResultsScreen()
    {
        if (resultsPanel == null) return;
        StartCoroutine(ShowResultsAfterDelay());
    }

    // ─────────────────────────────────────────────
    // Internal Helpers
    // ─────────────────────────────────────────────

    /// <summary>
    /// Rebuilds the task list UI.
    /// MODIFIED: Uses CurrentRoomTasks to only show tasks for the current room.
    /// </summary>
    private void RebuildTaskList()
    {
        if (taskListContent == null || taskRowPrefab == null) return;

        // Destroy all existing rows before rebuilding
        foreach (GameObject row in _taskRows)
            Destroy(row);
        _taskRows.Clear();

        // CHANGE 1: Use CurrentRoomTasks instead of Tasks
        // This ensures only tasks for the current room are displayed
        var tasks = TaskManager.Instance.CurrentRoomTasks;
        if (tasks == null) return;

        float yOffset = 0f;
        float rowHeight = 50f;
        float spacing = 5f;

        foreach (TaskEntry task in tasks)
        {
            GameObject row = Instantiate(taskRowPrefab, taskListContent);
            _taskRows.Add(row);

            // Manually position and size each row
            RectTransform rt = row.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.offsetMin = new Vector2(0, -(yOffset + rowHeight));
            rt.offsetMax = new Vector2(0, -yOffset);
            yOffset += rowHeight + spacing;

            // Set toggle state — non-interactable, reflects task completion
            Toggle toggle = row.GetComponentInChildren<Toggle>();
            if (toggle != null)
            {
                toggle.isOn = task.isCompleted;
                toggle.interactable = false;
            }

            // Set label text — completed tasks show [Done] prefix
            Transform labelTransform = row.transform.Find("Label");
            TMP_Text label = labelTransform?.GetComponent<TMP_Text>();
            Debug.Log($"[TaskUI] Label found: {label != null}, task: {task.title}");
            if (label != null)
                label.text = task.isCompleted ? $"[Done] {task.title}" : task.title;
        }
    }

    /// <summary>
    /// Updates the progress bar fill and text.
    /// MODIFIED: Uses CurrentRoomCompletedCount and CurrentRoomTaskCount for room-based progress.
    /// </summary>
    private void UpdateProgressBar()
    {
        // CHANGE 2: Use current room task counts instead of total counts
        // This shows progress only for the current room
        int completed = TaskManager.Instance.CurrentRoomCompletedCount;
        int total = TaskManager.Instance.CurrentRoomTaskCount;

        if (progressBarFill != null)
            progressBarFill.fillAmount = total > 0 ? (float)completed / total : 0f;

        if (progressText != null)
            progressText.text = $"{completed} / {total} Tasks Completed";
    }

    private IEnumerator ShowResultsAfterDelay()
    {
        // Wait 2 seconds so feedback panel for the last task has time to show
        yield return new WaitForSecondsRealtime(2f);

        // Hide feedback panel before showing results
        if (feedbackPanel != null) feedbackPanel.SetActive(false);
        if (_feedbackCoroutine != null)
        {
            StopCoroutine(_feedbackCoroutine);
            _feedbackCoroutine = null;
        }

        if (resultsText != null)
        {
            int total = TaskManager.Instance.TotalTaskCount;
            resultsText.text = $"Lesson Complete!\n\nYou completed all {total} tasks.\n\nWell done!";
        }

        resultsPanel.SetActive(true);

        // Play all done sound
        AudioManager.Instance?.PlayAllDone();

        StartCoroutine(HideResultsAfterDelay());
    }

    private IEnumerator HideFeedbackAfterDelay()
    {
        yield return new WaitForSecondsRealtime(feedbackDisplayDuration);
        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
        _feedbackCoroutine = null;
    }

    private IEnumerator HideResultsAfterDelay()
    {
        yield return new WaitForSecondsRealtime(resultsDisplayDuration);
        if (resultsPanel != null)
            resultsPanel.SetActive(false);
    }
}