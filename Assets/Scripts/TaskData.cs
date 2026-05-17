using System;
using System.Collections.Generic;

/// <summary>
/// TaskData defines all serializable data structures used by the task system.
/// These classes map directly to the JSON file format that instructors use to
/// configure learning activities without modifying the application code.
///
/// JSON file location: Assets/Resources/tasks.json
///
/// Example JSON format:
/// {
///   "lessonTitle": "Residential Construction Inspection",
///   "tasks": [
///     {
///       "id": "task_001",
///       "title": "Inspect the Sofa",
///       "description": "Click on the sofa to inspect its material.",
///       "targetElementId": "Sofa_LivingRoom",
///       "roomId": "LivingRoom",
///       "feedbackText": "Correct! This is an upholstered sofa.",
///       "isRequired": true
///     }
///   ]
/// }
///
/// The "targetElementId" field must match the "elementId" field set on a
/// BuildingElement component in the scene.
/// The "roomId" field determines which room this task belongs to.
/// </summary>

// Marks this class as serializable so Unity's JsonUtility can read/write it.
[Serializable]
public class TaskList
{
    /// <summary>
    /// The title of the lesson, displayed at the top of the task panel UI.
    /// </summary>
    public string lessonTitle = "Construction Inspection";

    /// <summary>
    /// The full list of tasks to be completed in this lesson.
    /// </summary>
    public List<TaskEntry> tasks = new List<TaskEntry>();
}

[Serializable]
public class TaskEntry
{
    /// <summary>
    /// Unique identifier for this task. Used to match against BuildingElement.elementId.
    /// Must be unique within the lesson. Example: "task_001", "task_wall_01".
    /// </summary>
    public string id;

    /// <summary>
    /// Short title shown in the task panel list. Keep it under ~40 characters.
    /// Example: "Inspect the Load-Bearing Wall"
    /// </summary>
    public string title;

    /// <summary>
    /// Longer description of what the student needs to do.
    /// Shown below the title in the task panel.
    /// </summary>
    public string description;

    /// <summary>
    /// The elementId of the BuildingElement the student must click to complete this task.
    /// Must exactly match the BuildingElement.elementId field on the target GameObject.
    /// </summary>
    public string targetElementId;

    /// <summary>
    /// The room this task belongs to (e.g., "LivingRoom", "Bedroom", "Outside").
    /// Tasks are only displayed when the player is in the matching room.
    /// Leave empty or use "Any" to show the task in all rooms.
    /// </summary>
    public string roomId = "";

    /// <summary>
    /// Text shown in the feedback panel after the student completes this task.
    /// Use this to explain what the element is and why it matters.
    /// </summary>
    public string feedbackText;

    /// <summary>
    /// If true, this task must be completed before the lesson is considered finished.
    /// If false, it is optional. Currently all tasks count toward progress equally.
    /// </summary>
    public bool isRequired = true;

    // ─── Runtime state (not serialized from JSON) ───────────────────────────

    /// <summary>
    /// Set to true at runtime when the student successfully completes this task.
    /// Not stored in the JSON file — this resets every time the scene loads.
    /// </summary>
    [NonSerialized]
    public bool isCompleted = false;
}
