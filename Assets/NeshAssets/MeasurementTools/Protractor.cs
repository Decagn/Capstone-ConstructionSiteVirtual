using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Protractor : MonoBehaviour, IMeasuringTool
{
    public string ToolName
    {
        get => "Protractor";
    }
    public Sprite ToolIcon { get; set; }
    /*
     * SYSTEM DESIGN
     * Protractor only measures angles.
     * It stores only three points in its SelectedPoints list.
     * We only update MeasuredAngles[0] because only the last measurement is stored.
     */
    public List<Vector3> SelectedPoints { get; set; }
    public List<float> MeasuredLengths { get; set; }
    public List<float> MeasuredAngles { get; set; }
    public event Action OnSelectedPointsUpdated;
    public event Action OnSelectedPointsReseted;

    public void Initialise(Sprite toolIcon)
    {
        ToolIcon = toolIcon;
        SelectedPoints = new List<Vector3>();
        MeasuredLengths = new List<float>();
        MeasuredAngles = new List<float>();
        MeasuredAngles.Add(0f);
    }

    public void HandleSelectedPoint(Vector3 selectedPoint)
    {
        int nSelectedPoints = SelectedPoints.Count;

        if (nSelectedPoints < 2)
        {
            AddSelectedPoint(selectedPoint);
        }
        else if (nSelectedPoints == 2)
        {
            MeasuredAngles[0] = GetAngle(SelectedPoints[0], SelectedPoints[1], selectedPoint);
            AddSelectedPoint(selectedPoint);
        }
        else if (nSelectedPoints == 3)
        {
            ResetSelectedPoints();
            AddSelectedPoint(selectedPoint);
        }
    }

    public void ResetSelectedPoints()
    {
        SelectedPoints.Clear();
        OnSelectedPointsReseted?.Invoke();
        Debug.Log("Protractor: Current selected points cleared.");
    }

    public void AddSelectedPoint(Vector3 selectedPoint)
    {
        SelectedPoints.Add(selectedPoint);
        OnSelectedPointsUpdated?.Invoke();
    }

    public void RemoveLastSelectedPoint()
    {
        if (SelectedPoints.Count > 0)
        {
            SelectedPoints.RemoveAt(SelectedPoints.Count - 1);
        }
        OnSelectedPointsUpdated?.Invoke();
    }

    private float GetAngle(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        float angle = MeasurementCalculator.AngleBetweenLines(pointA, pointB, pointC);
        Debug.Log($"Protractor: Angle between points --> {angle}");
        return angle;
    }
}
