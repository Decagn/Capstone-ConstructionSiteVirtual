/// <summary>
/// Utility classes for parsing and organising information derived from lesson plan JSON files.
/// </summary>

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Interface for JSON parsing, returns LessonPlan class populated with JSON data
public class JsonParser : MonoBehaviour
{
    public LessonPlan ParseLesson(string jsonStr)
    {
        return LessonPlan.CreateFromJSON(jsonStr);
    }        
};

// Class to be populated through parsing of lesson plan JSON file
[System.Serializable]
public class LessonPlan
{
    public string modelName;
    public Vector3 playerPos;
    public List<TextLabel> labels;
    public TaskList taskList;

    public static LessonPlan CreateFromJSON(string jsonStr)
    {
        return JsonUtility.FromJson<LessonPlan>(jsonStr);
    }
};

// Class to be populated as text label JSON data is parsed, implicitly called during LessonPlan construction
[System.Serializable]
public class TextLabel
{
    public Vector3 labelPos;
    public string text;
    public float fontSize;

    public static TextLabel CreateFromJSON(string jsonStr)
    {
        return JsonUtility.FromJson<TextLabel>(jsonStr);
    }
};

[System.Serializable]
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

    public static TaskList CreateFromJSON(string jsonStr)
    {
        return JsonUtility.FromJson<TaskList>(jsonStr);
    }
}

[System.Serializable]
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
    [System.NonSerialized]
    public bool isCompleted = false;

    public static TaskEntry CreateFromJSON(string jsonStr)
    {
        return JsonUtility.FromJson<TaskEntry>(jsonStr);
    }
}
