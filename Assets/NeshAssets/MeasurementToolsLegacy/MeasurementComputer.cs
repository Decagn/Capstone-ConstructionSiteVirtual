using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public static class MeasurementComputer
{
    public static float ComputeDistance(Vector3 pointA, Vector3 pointB)
    {
        return Vector3.Distance(pointA, pointB);
    }

    public static float ComputeAngle(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        Vector3 lineAB = (pointA - pointB);
        Vector3 lineBC = (pointB - pointC);

        return Vector3.Angle(lineAB, lineBC);
    }

    /*
     * OPTIMISATION
     * One function calculates both distances and angles so we won't have to iterate through the selected points list twice
     */
    /*
     * SYSTEM DESIGN
     * This function returns a list containing two lists:
     * List<float> distances at index 0 and
     * List<float> angles at index 1
     */
    public static List<List<float>> ComputeDistancesAndAngles(List<Vector3> selectedPoints)
    {
        List<float> distances = new List<float>();
        List<float> angles = new List<float>();

        for (int i = 0;  i < selectedPoints.Count - 1; i++)
        {
            distances[i] = ComputeDistance(selectedPoints[i], selectedPoints[i + 1]);
            if (i < selectedPoints.Count - 2)
            {
                angles[i] = ComputeAngle(selectedPoints[i], selectedPoints[i + 1], selectedPoints[i + 2]);
            }  
        }

        List<List<float>> measurements = new List<List<float>>();
        measurements.Add(distances);
        measurements.Add(angles);

        return measurements;
    }
}
