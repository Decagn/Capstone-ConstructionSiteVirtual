using UnityEngine;

public class MeasurementCalculator
{
    public static float DistanceBetweenPoints(Vector3 pointA, Vector3 pointB)
    {
        return Vector3.Distance(pointA, pointB);
    }

    public static float AngleBetweenLines(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        Vector3 lineAB = (pointB - pointA).normalized;
        Vector3 lineBC = (pointB - pointC).normalized;
        return Vector3.Angle(lineBC, lineAB);
    }
}
