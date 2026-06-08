using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public static class InlineSnapping
{
    // Returns a list of points that are on the vertical line or horizontal plane of the other points in the scene.
    public static List<Vector3> GetPoints(Vector3 point, List<Vector3> otherPoints)
    {
        List<Vector3> inlinePoints = new List<Vector3>();

        foreach (Vector3 otherPoint in otherPoints)
        {
            inlinePoints.Add(new Vector3(otherPoint.x, point.y, otherPoint.z));
            inlinePoints.Add(new Vector3(point.x, otherPoint.y, point.z));
        }

        return inlinePoints;
    }
}
