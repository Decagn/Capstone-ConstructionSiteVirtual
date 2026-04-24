using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public static class PointSelectorL
{
    /*
     * [SYSTEM DESIGN]
     * Max ray length determines how far in the scene can points be selected.
     * Adjust to a distance that makes sense in terms of how far we should be able to measure.
     */
    private const float _maxRayLength = Mathf.Infinity;

    public static Vector3 TrySelectPoint(Vector2 pointOnScreen, List<Vector3> selectedPoints, bool enableSnapping, float SnapDistance)
    {
        Ray ray = Camera.main.ScreenPointToRay(pointOnScreen);
        bool surfaceFound = Physics.Raycast(ray, out RaycastHit hit, _maxRayLength);

        Vector3 point = hit.point;
        if (point == Vector3.zero || !enableSnapping ) return point;
        
        List<Vector3> closestPoints = new List<Vector3>();
        closestPoints.Add(SnapToLine(point, selectedPoints));
        Vector3 closestPoint = FindClosestPoint(point, closestPoints);

        bool inSnappingRange = Vector3.Distance(point, closestPoint) <= SnapDistance;
        return inSnappingRange ? closestPoint : point;
    }

    private static Vector3 SnapToLine(Vector3 point, List<Vector3> previousPoints)
    {
        List<Vector3> inLinePoints = new List<Vector3>();

        foreach (Vector3 previousPoint in previousPoints)
        {
            float x = previousPoint.x;
            float y = previousPoint.y;
            float z = previousPoint.z;

            inLinePoints.Add(new Vector3(x, point.y, z));
            inLinePoints.Add(new Vector3(point.x, y, point.z));
        }

        return FindClosestPoint(point, inLinePoints);
    }

    private static Vector3 FindClosestPoint(Vector3 point, List<Vector3> candidatePoints)
    {
        Vector3 closest = point;
        float distance = Mathf.Infinity;

        foreach (Vector3 candidate in candidatePoints)
        {
            float candidateDisance = Vector3.Distance(candidate, point);
            if (candidateDisance < distance)
            {
                distance = candidateDisance;
                closest = candidate;
            }
        }

        return closest;
    }

    //private static void EdgeDetection(RaycastHit hit)
    //{
    //    Mesh mesh = GetMesh(hit);
    //    if (mesh == null) return;

    //    Transform meshTransform = hit.collider.transform;
    //    Vector3[] vertices = mesh.vertices;
    //    int[] triangles = mesh.triangles;

    //    for (int t = 0; t < triangles.Length; t += 3)
    //    {
    //        Vector3[] triVerts = new Vector3[]
    //            {
    //                meshTransform.TransformPoint(vertices[triangles[t]]),
    //                meshTransform.TransformPoint(vertices[triangles[t + 1]]),
    //                meshTransform.TransformPoint(vertices[triangles[t + 2]])
    //            };

    //        Debug.Log($"Vertices: {triVerts[0]}, {triVerts[1]}, {triVerts[2]}");
    //    }



    //}

    //private static Mesh GetMesh(RaycastHit hit)
    //{
    //    /*
    //     * [SYSTEM DESGIN]
    //     * Default to using the mesh filter because it is visually accurate
    //     * otherwise fall back on the mesh collider.
    //     */
    //    MeshFilter filter = hit.collider.GetComponent<MeshFilter>();
    //    if (filter != null) return filter.sharedMesh;

    //    MeshCollider collider = hit.collider as MeshCollider;
    //    if (collider != null) return collider.sharedMesh;

    //    return null;
    //}
}
