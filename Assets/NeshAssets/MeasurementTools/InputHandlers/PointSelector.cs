using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public static class PointSelector
{
    /*
     * [SYSTEM DESIGN]
     * Max ray length determines how far in the scene can points be selected.
     * Adjust to a distance that makes sense in terms of how far we should be able to measure.
     */
    private const float _maxRayLength = Mathf.Infinity;

    public static Vector3 TrySelectPoint(Vector2 pointOnScreen)
    {
        Ray ray = Camera.main.ScreenPointToRay(pointOnScreen);
        bool surfaceFound = Physics.Raycast(ray, out RaycastHit hit, _maxRayLength);
        if (hit.point == Vector3.zero) return hit.point;

        EdgeDetection(hit);

        return hit.point;
    }

    private static void EdgeDetection(RaycastHit hit)
    {
        Mesh mesh = GetMesh(hit);
        if (mesh == null) return;

        int[] triangles = mesh.triangles;

        Debug.Log(triangles[0]);

    }

    private static Mesh GetMesh(RaycastHit hit)
    {
        /*
         * [SYSTEM DESGIN]
         * Default to using the mesh filter because it is visually accurate
         * otherwise fall back on the mesh collider.
         */
        MeshFilter filter = hit.collider.GetComponent<MeshFilter>();
        if (filter != null) return filter.sharedMesh;

        MeshCollider collider = hit.collider as MeshCollider;
        if (collider != null) return collider.sharedMesh;

        return null;
    }
}
