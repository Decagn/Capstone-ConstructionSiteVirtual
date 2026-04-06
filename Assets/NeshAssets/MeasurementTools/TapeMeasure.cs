using System.Collections.Generic;
using UnityEngine;

public class TapeMeasure : MonoBehaviour, IMeasuringTool
{
    public string ToolName => "TapeMeasure";
    public List<Vector3> SelectedPoints = new List<Vector3>();

    public void HandleSelectedPoint(Vector3 selectedPoint)
    {
        int nSelectedPoints = SelectedPoints.Count;

        if (nSelectedPoints == 0)
        {
            AddSelectedPoint(selectedPoint);
        }
        else if (nSelectedPoints == 1)
        {
            AddSelectedPoint(selectedPoint);
            GetDistance(selectedPoint, SelectedPoints[0]);
        }
        else if (nSelectedPoints == 2)
        {
            ResetSelectedPoints();
            AddSelectedPoint(selectedPoint);
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
