using UnityEngine;

/// <summary>
/// BuildingElement - Stores information about a structural building component
/// and notifies the TaskManager when the player interacts with it.
///
/// Attach this script to any interactable building component in the scene
/// (e.g. walls, floors, roof panels, foundations).
///
/// The elementId field must match the "targetElementId" value in tasks.json
/// for this element to count toward task completion.
///
/// Interaction is triggered externally by CTrainerInteraction.cs (desktop)
/// or the mobile equivalent, which calls OnClick() via Raycast hit.
/// </summary>

public class BuildingElement : MonoBehaviour
{
    [Header("Element Info")]
    [Tooltip("Display name of this structural element. Shown in console and UI.")]
    public string elementName;

    [Tooltip("Primary material of this element (e.g. Brick, Concrete, Timber).")]
    public string material;

    [Tooltip("Longer description of this element's role in the building.")]
    [TextArea]
    public string description;

    [Header("Task System")]
    [Tooltip("Unique identifier used to match this element against tasks in tasks.json. " +
             "Must exactly match the 'targetElementId' field in the JSON task entry. " +
             "Example: 'Wall_LoadBearing', 'Roof_Truss', 'Foundation_Slab'")]
    public string elementId;

    /// <summary>
    /// Called by CTrainerInteraction (desktop) or the mobile interaction script
    /// when the player clicks or taps on this element.
    ///
    /// Logs element info to the Console and reports the click to TaskManager
    /// so the task system can check if this element completes any active task.
    /// </summary>
    public void OnClick()
    {
        // Log element info to the Console for debugging and development visibility.
        Debug.Log($"[Element] {elementName} | Material: {material} | {description}");

        // Notify the TaskManager that this element was clicked.
        // TaskManager will check if this elementId matches any task's targetElementId
        // and mark it complete if it does.
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.OnElementClicked(elementId, elementName);
        }
        else
        {
            // TaskManager not in scene — task system is not set up yet.
            // This is fine during development before the task system is integrated.
            Debug.LogWarning("[BuildingElement] TaskManager not found in scene. " +
                             "Task completion will not be tracked.");
        }
    }
}
