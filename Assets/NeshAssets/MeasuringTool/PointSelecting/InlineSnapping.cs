using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public static class InlineSnapping
{
    public static List<Vector3> GetPoints(Vector3 point, List<Vector3> otherPoints)
    {
        List<Vector3> inlinePoints = new List<Vector3>();

        foreach (Vector3 otherP in otherPoints)
        {
            inlinePoints.Add(new Vector3(otherP.x, point.y, otherP.z));
            inlinePoints.Add(new Vector3(point.x, otherP.y, point.z));
        }

        return inlinePoints;
    }
}
