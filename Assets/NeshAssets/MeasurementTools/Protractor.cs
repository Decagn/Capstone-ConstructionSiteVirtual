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

        if (nSelectedPoints == 0 || nSelectedPoints == 1)
        {
            AddSelectedPoint(selectedPoint);
            OnSelectedPointsUpdated?.Invoke();
        }
        else if (nSelectedPoints == 2)
        {
            AddSelectedPoint(selectedPoint);
            MeasuredAngles[0] = GetAngle(SelectedPoints[0], SelectedPoints[1], SelectedPoints[2]);
            OnSelectedPointsUpdated?.Invoke();
        }
        else if (nSelectedPoints == 3)
        {
            ResetSelectedPoints();
            AddSelectedPoint(selectedPoint);
            OnSelectedPointsUpdated?.Invoke();
        }
    }

    public void ResetSelectedPoints()
    {
        SelectedPoints.Clear();
        Debug.Log("Protractor: Current selected points cleared.");
    }

    public void AddSelectedPoint(Vector3 selectedPoint)
    {
        SelectedPoints.Add(selectedPoint);
    }

    private float GetAngle(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        float angle = MeasurementCalculator.AngleBetweenLines(pointA, pointB, pointC);
        Debug.Log($"Protractor: Angle between points --> {angle}");
        return angle;
    }
}
