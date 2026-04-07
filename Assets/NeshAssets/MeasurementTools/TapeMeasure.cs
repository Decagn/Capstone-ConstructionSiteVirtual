using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class TapeMeasure : MonoBehaviour, IMeasuringTool
{
    public string ToolName
    {
        get => "Tape Measure";
    }
    public Sprite ToolIcon { get; set; }
    /*
     * SYSTEM DESIGN
     * TapeMeasure only measures lengths.
     * It stores only two points in its SelectedPoints list.
     * We only update MeasuredLengths[0] because only the last measurement is stored.
     */
    public List<Vector3> SelectedPoints { get; set; }
    public List<float> MeasuredLengths { get; set; }
    public List<float> MeasuredAngles { get; set; }
    public event Action OnSelectedPointsUpdated;

    public void Initialise(Sprite toolIcon)
    {
        ToolIcon = toolIcon;
        SelectedPoints = new List<Vector3>();
        MeasuredLengths = new List<float>();
        MeasuredLengths.Add(0f);
    }

    public void HandleSelectedPoint(Vector3 selectedPoint)
    {
        int nSelectedPoints = SelectedPoints.Count;

        if (nSelectedPoints == 0)
        {
            AddSelectedPoint(selectedPoint);
            OnSelectedPointsUpdated?.Invoke();
        }
        else if (nSelectedPoints == 1)
        {
            AddSelectedPoint(selectedPoint);
            MeasuredLengths[0] = GetDistance(selectedPoint, SelectedPoints[0]);
            OnSelectedPointsUpdated?.Invoke();
        }
        else if (nSelectedPoints == 2)
        {
            ResetSelectedPoints();
            AddSelectedPoint(selectedPoint);
            OnSelectedPointsUpdated?.Invoke();
        }
    }

    public void ResetSelectedPoints()
    {
        SelectedPoints.Clear();
        Debug.Log("TapeMeasure: Current selected points cleared.");
    }

    public void AddSelectedPoint(Vector3 selectedPoint)
    {
        SelectedPoints.Add(selectedPoint);
    }

    private float GetDistance(Vector3 pointA, Vector3 pointB)
    {
        float distance = MeasurementCalculator.DistanceBetweenPoints(pointA, pointB);
        Debug.Log($"TapeMeasure: Distance between points --> {distance}");
        return distance;
    }
}
