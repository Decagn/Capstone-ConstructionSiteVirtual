using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LessonConfig defines the complete structure for a construction learning lesson.
/// This includes room definitions (coordinates, radius) and their associated tasks.
/// All data is loaded from a single JSON file in Resources/.
///
/// File location: Assets/Resources/lesson_config.json
///
/// Example JSON format:
/// {
///   "lessonTitle": "Construction Site Learning",
///   "rooms": [
///     {
///       "roomId": "LivingRoom",
///       "roomName": "Living Room",
///       "centerPosition": { "x": 164, "y": -37, "z": 336 },
///       "detectionRadius": 100,
///       "tasks": [
///         {
///           "id": "task_001",
///           "title": "Inspect Sofa",
///           "targetElementId": "Sofa_LivingRoom",
///           ...
///         }
///       ],
///       "measurements": [
///         {
///             "id": "table_counter",
///             "label": "Table Counter",
///             "points": [
///                 { "x": 1.00, "y": 0.92, "z": -2.65 },
///                 { "x": 1.89, "y": 0.92, "z": -2.65 },
///                 { "x": 1.89, "y": 0.92, "z": -0.15 },
///                 { "x": 1.00, "y": 0.92, "z": -0.15 }
///                 ]
///             }
//          ]
///     }
///   ]
/// }
/// </summary>

[Serializable]
public class LessonConfig
{
    /// <summary>
    /// The title of this lesson, displayed in the UI.
    /// </summary>
    public string lessonTitle = "Construction Learning";

    /// <summary>
    /// List of all rooms in this lesson, each with its own tasks.
    /// </summary>
    public List<RoomConfig> rooms = new List<RoomConfig>();
}

[Serializable]
public class RoomConfig
{
    /// <summary>
    /// Unique identifier for this room (e.g., "LivingRoom", "Bedroom").
    /// Used to match player position against room boundaries.
    /// </summary>
    public string roomId;

    /// <summary>
    /// Display name shown in the UI (e.g., "Living Room", "Master Bedroom").
    /// </summary>
    public string roomName;

    /// <summary>
    /// World position of the room's center point in Unity coordinates.
    /// Used as the origin for distance-based room detection.
    /// </summary>
    public Vector3 centerPosition;

    /// <summary>
    /// Detection radius in Unity units.
    /// Player is considered "in this room" if within this distance from centerPosition.
    /// </summary>
    public float detectionRadius = 50f;

    /// <summary>
    /// List of tasks that belong to this room.
    /// These tasks are only shown when the player is in this room.
    /// </summary>
    public List<TaskEntry> tasks = new List<TaskEntry>();

    /// <summary>
    /// List of measurement tasks that belong to this room.
    /// These tasks are only shown when the player is in this room.
    /// </summary>
    public List<LessonMeasurement> measurements = new List<LessonMeasurement>();

    /// <summary>
    /// Convert this RoomConfig to a RoomData for RoomDetector.
    /// </summary>
    public RoomData ToRoomData()
    {
        return new RoomData
        {
            roomId = this.roomId,
            roomName = this.roomName,
            centerPosition = centerPosition,
            detectionRadius = this.detectionRadius
        };
    }
}

[Serializable]
public class LessonMeasurement
{
    public string id;
    public string label;
    public List<Vector3> points;
}

/// <summary>
/// TaskEntry remains the same as before, but now lives inside a RoomConfig.
/// </summary>
[Serializable]
public class TaskEntry
{
    public string id;
    public string title;
    public string description;
    public string targetElementId;
    public string measurementId;
    public string feedbackText;
    public bool isRequired = true;

    // Runtime state - not serialized from JSON
    [NonSerialized]
    public bool isCompleted = false;

    // Room ID is now determined by which RoomConfig contains this task
    // No need for a separate roomId field
}
