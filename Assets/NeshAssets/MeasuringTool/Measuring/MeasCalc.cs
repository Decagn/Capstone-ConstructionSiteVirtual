using UnityEngine;

public static class MeasCalc
{
    public static float GetLength(Vector3 pointA, Vector3 pointB) => Vector3.Distance(pointA, pointB);

    public static float GetAngle(Vector3 pointA, Vector3 vertex, Vector3 pointB)
    {
        Vector3 lineA = (pointA - vertex).normalized;
        Vector3 lineB = (pointB - vertex).normalized;
        return Vector3.Angle(lineA, lineB);
    }
}
