using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// RoomDetector monitors the player's position and determines which room they are currently in.
/// Uses distance-based detection: each room has a center point and detection radius.
/// When the player enters a new room, it notifies the TaskManager to update the task display.
///
/// Setup in Unity Editor:
///   1. Attach this script to the Player GameObject (or TaskManager GameObject).
///   2. Assign the playerTransform field to the Player's Transform.
///   3. Room data is configured in the Inspector via the 'rooms' list.
///   4. Alternatively, rooms can be defined in code (see InitializeRoomsFromCode example).
///
/// Room detection priority:
///   - If the player is within multiple room radii, the closest room center wins.
///   - If the player is outside all room radii, currentRoom becomes "Outside" or null.
/// </summary>
public class RoomDetector : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Inspector Settings
    // ─────────────────────────────────────────────

    [Header("Player Reference")]
    [Tooltip("Reference to the Player GameObject's Transform. Used to check player position.")]
    public Transform playerTransform;

    [Header("Room Definitions")]
    [Tooltip("List of all rooms in the scene. Each room has a center position and detection radius.")]
    public List<RoomData> rooms = new List<RoomData>();

    [Header("Detection Settings")]
    [Tooltip("How often to check player position (in seconds). Lower = more responsive, higher = better performance.")]
    public float detectionInterval = 0.2f;

    [Tooltip("Room ID to use when player is outside all defined rooms. Leave empty for null.")]
    public string outsideRoomId = "Outside";

    // ─────────────────────────────────────────────
    // Runtime State
    // ─────────────────────────────────────────────

    // The room ID the player is currently in (e.g., "LivingRoom", "Bedroom", "Outside")
    private string _currentRoomId = null;

    // Timer for detection interval
    private float _detectionTimer = 0f;

    // ─────────────────────────────────────────────
    // Properties
    // ─────────────────────────────────────────────

    /// <summary>
    /// The room ID the player is currently in.
    /// Returns null if player is outside all defined rooms and outsideRoomId is empty.
    /// </summary>
    public string CurrentRoomId => _currentRoomId;

    /// <summary>
    /// The display name of the current room (e.g., "Living Room").
    /// Returns "Outside" or "Unknown" if currentRoomId is null/empty.
    /// </summary>
    public string CurrentRoomName
    {
        get
        {
            if (string.IsNullOrEmpty(_currentRoomId))
                return "Unknown";

            RoomData room = rooms.FirstOrDefault(r => r.roomId == _currentRoomId);
            return room != null ? room.roomName : _currentRoomId;
        }
    }

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Start()
    {
        // Automatically find the player if not assigned
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
            else
                Debug.LogError("[RoomDetector] No playerTransform assigned and no GameObject with 'Player' tag found.");
        }

        // Initialize rooms from code if the Inspector list is empty
        // Comment this out if you prefer to configure rooms in the Inspector
        if (rooms.Count == 0)
        {
            InitializeRoomsFromCode();
        }

        // Perform initial room detection
        DetectCurrentRoom();
    }

    private void Update()
    {
        // Skip detection if player reference is missing
        if (playerTransform == null)
            return;

        // Update detection timer
        _detectionTimer += Time.deltaTime;

        // Check room detection at the specified interval
        if (_detectionTimer >= detectionInterval)
        {
            _detectionTimer = 0f;
            DetectCurrentRoom();
        }
    }

    // ─────────────────────────────────────────────
    // Room Detection Logic
    // ─────────────────────────────────────────────

    /// <summary>
    /// Checks the player's current position against all defined rooms.
    /// Updates currentRoomId if the player has entered a new room.
    /// Notifies TaskManager when the room changes.
    /// </summary>
    private void DetectCurrentRoom()
    {
        Vector3 playerPos = playerTransform.position;
        string newRoomId = null;
        float closestDistance = float.MaxValue;

        // Check each room to see if the player is within its radius
        foreach (RoomData room in rooms)
        {
            float distance = Vector3.Distance(playerPos, room.centerPosition);

            // Player is within this room's detection radius
            if (distance <= room.detectionRadius)
            {
                // If multiple rooms overlap, choose the one with the closest center
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    newRoomId = room.roomId;
                }
            }
        }

        // If player is outside all rooms, use the outsideRoomId (if configured)
        if (newRoomId == null && !string.IsNullOrEmpty(outsideRoomId))
        {
            newRoomId = outsideRoomId;
        }

        // Room changed — notify TaskManager
        if (newRoomId != _currentRoomId)
        {
            string previousRoom = _currentRoomId;
            _currentRoomId = newRoomId;

            Debug.Log($"[RoomDetector] Player entered room: {CurrentRoomName} (ID: {_currentRoomId})");

            // Notify TaskManager to update the task display for the new room
            if (TaskManager.Instance != null)
            {
                TaskManager.Instance.OnRoomChanged(_currentRoomId);
            }
        }
    }

    // ─────────────────────────────────────────────
    // Room Configuration
    // ─────────────────────────────────────────────

    /// <summary>
    /// Example: Initialize rooms from code instead of the Inspector.
    /// You can replace this with your actual room positions and radii.
    /// Uncomment the Start() call to use this.
    /// </summary>
    private void InitializeRoomsFromCode()
    {
        rooms.Clear();

        // Living Room
        rooms.Add(new RoomData
        {
            roomId = "LivingRoom",
            roomName = "Living Room",
            centerPosition = new Vector3(164f, -37f, 336f),
            detectionRadius = 100f
        });

        // Outside
        rooms.Add(new RoomData
        {
            roomId = "Outside",
            roomName = "Outside",
            centerPosition = new Vector3(-176f, -37f, 318f),
            detectionRadius = 150f
        });

        Debug.Log($"[RoomDetector] Initialized {rooms.Count} rooms from code.");
    }

    // ─────────────────────────────────────────────
    // Public API
    // ─────────────────────────────────────────────

    /// <summary>
    /// Manually force a room detection check.
    /// Useful for debugging or when teleporting the player.
    /// </summary>
    public void ForceDetection()
    {
        DetectCurrentRoom();
    }

    /// <summary>
    /// Get the RoomData for a specific room ID.
    /// Returns null if the room is not found.
    /// </summary>
    public RoomData GetRoomData(string roomId)
    {
        return rooms.FirstOrDefault(r => r.roomId == roomId);
    }

    // ─────────────────────────────────────────────
    // Debug Visualization (Editor Only)
    // ─────────────────────────────────────────────

#if UNITY_EDITOR
    /// <summary>
    /// Draw room detection radii in the Scene view for debugging.
    /// Each room is shown as a green wireframe sphere.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (rooms == null || rooms.Count == 0)
            return;

        foreach (RoomData room in rooms)
        {
            // Draw room detection radius as a green wireframe sphere
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(room.centerPosition, room.detectionRadius);

            // Draw room center as a small yellow sphere
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(room.centerPosition, 2f);
        }

        // Draw player position if assigned
        if (playerTransform != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(playerTransform.position, 1.5f);
        }
    }
#endif
}

/// <summary>
/// RoomData defines a single room's detection zone.
/// Each room has a unique ID, display name, center position, and detection radius.
/// </summary>
[System.Serializable]
public class RoomData
{
    [Header("Room Identity")]
    [Tooltip("Unique identifier for this room (e.g., 'LivingRoom', 'Bedroom', 'Kitchen').")]
    public string roomId;

    [Tooltip("Display name shown in the UI (e.g., 'Living Room', 'Master Bedroom').")]
    public string roomName;

    [Header("Detection Zone")]
    [Tooltip("World position of the room's center point. Used as the detection zone origin.")]
    public Vector3 centerPosition;

    [Tooltip("Detection radius in Unity units. Player is 'in this room' if within this distance from centerPosition.")]
    public float detectionRadius = 50f;
}
