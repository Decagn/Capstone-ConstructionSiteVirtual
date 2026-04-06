using UnityEngine;

public class MeasurementCalculator
{
    public static float DistanceBetweenPoints(Vector3 pointA, Vector3 pointB)
    {
        return Vector3.Distance(pointA, pointB);
    }
}
