using System.Collections.Generic;
using UnityEngine;

/*
 * Calculator
 * A static class that performs calculations needed for the measurement tools.
 */
public static class Calculator
{
    public static float GetLength(Vector3 pointA, Vector3 pointB) => Vector3.Distance(pointA, pointB);

    public static float GetAngle(Vector3 pointA, Vector3 vertex, Vector3 pointB)
    {
        Vector3 lineA = (pointA - vertex).normalized;
        Vector3 lineB = (pointB - vertex).normalized;
        return Vector3.Angle(lineA, lineB);
    }

    private static float GetTriangleArea(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        Vector3 lineA = pointB - pointA;
        Vector3 lineC = pointC - pointA;

        return Vector3.Cross(lineA, lineC).magnitude * 0.5f;
    }

    public static float GetAreaOfPolygon(List<Vector3> points)
    {
        if (points.Count < 3) DebugMeasurementTools.Log("Not enough points to form polygon", "Calculator");
        float area = 0;

        Vector3 firstVertex = points[0];
        Vector3 secondVertex = points[1];

        for (int v = 2; v < points.Count; v++)
        {
            float newArea = GetTriangleArea(firstVertex, secondVertex, points[v]);
            area += newArea;

            secondVertex = points[v];
        }

        return area;
    }
}
