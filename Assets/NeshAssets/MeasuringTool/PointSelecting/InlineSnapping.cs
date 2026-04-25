using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public static class InlineSnapping
{
    public static List<Vector3> GetPoint(Vector3 point, List<Vector3> otherPoints)
    {
        List<Vector3> inlinePoints = new List<Vector3>();

        foreach (Vector3 otherP in otherPoints)
        {
            float x = otherP.x;
            float y = otherP.y;
            float z = otherP.z;

            inlinePoints.Add(new Vector3(x, point.y, z));
            inlinePoints.Add(new Vector3(point.x, y, point.z));
        }

        return inlinePoints;
    }
}
