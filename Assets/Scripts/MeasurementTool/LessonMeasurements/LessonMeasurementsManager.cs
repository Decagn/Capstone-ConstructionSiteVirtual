using System.Collections.Generic;
using UnityEngine;

/*
 * Lesson Measurements Manager
 * Receives measurement data from LessonConfig (via LessonLoader) and displays
 * the measurements for the current room via LessonMeasurementsPointsVisualiser.
 */
public class LessonMeasurementsManager : MonoBehaviour
{
    // Visualises the lesson measurement points and lines in the world.
    [SerializeField] private LessonMeasurementsPointsVisualiser _visualiser;

    // List of rooms in the Lesson, each room has different measuring tasks.
    private List<RoomConfig> _rooms = new List<RoomConfig>();

    // List of measurements for the current room.
    private List<List<Vector3>> _currentMeasurements = new List<List<Vector3>>();

    // Called by LessonLoader when a new lesson is loaded.
    // Stores all room configs and clears any existing visuals.
    public void LoadMeasurements(List<RoomConfig> rooms)
    {
        _rooms = new List<RoomConfig>();
        if (rooms != null)
            _rooms = rooms;

        _currentMeasurements.Clear();
        _visualiser.ClearMeasurements();
    }

    // Called by TaskManager when the player enters a new room.
    // Swaps the displayed measurements to those belonging to the new room.
    public void OnRoomChanged(string roomId)
    {
        _currentMeasurements.Clear();
        _visualiser.ClearMeasurements();

        RoomConfig room = null;
        foreach (RoomConfig r in _rooms)
        {
            if (r.roomId == roomId)
            {
                room = r;
                break;
            }
        }

        if (room == null || room.measurements == null || room.measurements.Count == 0)
            return;

        foreach (LessonMeasurement measurement in room.measurements)
        {
            // Skip invalid measurements.
            if (measurement.points == null || measurement.points.Count < 2)
                continue;

            // Close the polygon by appending the first point at the end.
            List<Vector3> points = new List<Vector3>(measurement.points);
            points.Add(points[0]);

            _currentMeasurements.Add(points);
        }

        _visualiser.DisplayLessonMeasurements(_currentMeasurements);
    }

    // Used by PointSelector for snapping.
    public List<Vector3> GetCurrentMeasurementPoints()
    {
        List<Vector3> points = new List<Vector3>();

        foreach (List<Vector3> measurement in _currentMeasurements)
            foreach (Vector3 point in measurement)
                points.Add(point);

        return points;
    }

    // Checks whether the list of player points matches a named measurement
    // Used by TaskManager to validate measuring tasks.
    public bool IsMeasurementComplete(string measurementId, List<Vector3> playerPoints, float tolerance = 0f)
    {
        // Searches every room's measurement list
        // to find which room contains a measurement with the matching measurementId.
        RoomConfig room = null;
        foreach (RoomConfig r in _rooms)
        {
            if (r.measurements == null) 
                continue;
            foreach (LessonMeasurement m in r.measurements)
            {
                if (m.id == measurementId)
                {
                    room = r;
                    break;
                }
            }
            if (room != null) 
                break;
        }

        if (room == null)
            return false;

        // Searches the found room's measurements again
        // to grab the actual LessonMeasurement object with the matching measurementId.
        LessonMeasurement target = null;
        foreach (LessonMeasurement m in room.measurements)
        {
            if (m.id == measurementId)
            {
                target = m;
                break;
            }
        }

        if (playerPoints.Count != target.points.Count)
            return false;

        for (int i = 0; i < target.points.Count; i++)
        {
            if (Vector3.Distance(playerPoints[i], target.points[i]) > tolerance)
                return false;
        }

        return true;
    }
}