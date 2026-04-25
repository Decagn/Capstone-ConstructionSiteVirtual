using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PointSelector : MonoBehaviour
{
    [SerializeField] float _furthestPoint = Mathf.Infinity;

    [SerializeField] bool _snapping = true;
    [SerializeField] float _snapDist = 0.5f;
    [SerializeField] bool _inlineSnapping = true;

    public Vector3 GetPoint(Vector2 screenPoint, List<Vector3> selecPoints)
    {
        (Vector3 point, RaycastHit hit) = TryGetPoint(screenPoint);

        if (!_snapping || selecPoints.Count < 0 ) return point;

        Vector3 snapPoint = GetSnapPoint(point, selecPoints);
        return snapPoint;
    }

    private (Vector3 point, RaycastHit ray) TryGetPoint(Vector2 screenPoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);
        Physics.Raycast(ray, out RaycastHit hit, _furthestPoint);
        return (hit.point, hit);
    }

    private Vector3 GetSnapPoint(Vector3 point, List<Vector3> prevSelecPoints)
    {
        List<Vector3> snapPoints = new List<Vector3>();

        List<Vector3> inlinePoints = InlineSnapping.GetPoint(point, prevSelecPoints);
        snapPoints.AddRange(inlinePoints);

        Vector3 snapPoint = FindClosestPoint(point, snapPoints);
        bool inSnappingRange = Vector3.Distance(point, snapPoint) <= _snapDist;

        return inSnappingRange ? snapPoint : point;
    }

    public static Vector3 FindClosestPoint(Vector3 point, List<Vector3> candidatePoints)
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
}
