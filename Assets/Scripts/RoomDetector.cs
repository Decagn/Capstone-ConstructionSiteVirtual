using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// RoomDetector monitors the player's position and determines which room they are currently in.
/// 
/// NEW: Room configurations can now be loaded from JSON via TaskManager.
/// Supports both manual Inspector configuration and automatic file loading.
///
/// Usage modes:
///   1. Manual: Configure rooms in the Inspector (original behavior)
///   2. Automatic: Rooms are loaded from lesson_config.json via TaskManager
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
    [Tooltip("List of all rooms in the scene. Can be configured manually or loaded from JSON.")]
    public List<RoomData> rooms = new List<RoomData>();

    [Header("Detection Settings")]
    [Tooltip("How often to check player position (in seconds). Lower = more responsive, higher = better performance.")]
    public float detectionInterval = 0.2f;

    [Tooltip("Room ID to use when player is outside all defined rooms. Leave empty for null.")]
    public string outsideRoomId = "Outside";

    [Header("Configuration Mode")]
    [Tooltip("If true, rooms will be loaded from JSON file via TaskManager. If false, use Inspector configuration.")]
    public bool loadFromFile = true;

    // ─────────────────────────────────────────────
    // Runtime State
    // ─────────────────────────────────────────────

    private string _currentRoomId = null;
    private float _detectionTimer = 0f;
    private bool _roomsLoaded = false;

    // ─────────────────────────────────────────────
    // Properties
    // ─────────────────────────────────────────────

    public string CurrentRoomId => _currentRoomId;

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

        // If not loading from file, rooms should already be configured in Inspector
        if (!loadFromFile && rooms.Count > 0)
        {
            _roomsLoaded = true;
            Debug.Log($"[RoomDetector] Using {rooms.Count} rooms from Inspector configuration.");
            DetectCurrentRoom();
        }
        // If loading from file, wait for TaskManager to call LoadRoomsFromConfig()
        else if (loadFromFile)
        {
            Debug.Log("[RoomDetector] Waiting for room configuration from TaskManager...");
        }
        // No rooms configured and not loading from file
        else
        {
            Debug.LogWarning("[RoomDetector] No rooms configured and loadFromFile is false.");
        }
    }

    private void Update()
    {
        // Skip detection if rooms haven't been loaded yet
        if (!_roomsLoaded || playerTransform == null)
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
    // Public API
    // ─────────────────────────────────────────────

    /// <summary>
    /// Load room configurations from TaskManager.
    /// This is called automatically by TaskManager after loading lesson_config.json.
    /// </summary>
    /// <param name="roomDataList">List of room configurations from JSON</param>
    public void LoadRoomsFromConfig(List<RoomData> roomDataList)
    {
        if (roomDataList == null || roomDataList.Count == 0)
        {
            Debug.LogWarning("[RoomDetector] LoadRoomsFromConfig called with empty room list.");
            return;
        }

        rooms = roomDataList;
        _roomsLoaded = true;

        Debug.Log($"[RoomDetector] Loaded {rooms.Count} rooms from configuration file.");

        // Perform initial room detection
        DetectCurrentRoom();
    }

    /// <summary>
    /// Manually force a room detection check.
    /// </summary>
    public void ForceDetection()
    {
        DetectCurrentRoom();
    }

    /// <summary>
    /// Get the RoomData for a specific room ID.
    /// </summary>
    public RoomData GetRoomData(string roomId)
    {
        return rooms.FirstOrDefault(r => r.roomId == roomId);
    }

    // ─────────────────────────────────────────────
    // Room Detection Logic
    // ─────────────────────────────────────────────

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
}

/// <summary>
/// RoomData defines a single room's detection zone.
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
