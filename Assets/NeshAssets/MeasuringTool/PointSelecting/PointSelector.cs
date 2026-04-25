using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PointSelector : MonoBehaviour
{
    [SerializeField] private float _furthestPoint = Mathf.Infinity;

    [SerializeField] private bool _snapping = true;

    [SerializeField] private bool _snapToPreviousPoint = true;
    [SerializeField] private float _snapToPreviousPointDistance = 0.5f;

    [SerializeField] private bool _snapToInlinePoint = true;
    [SerializeField] private float _snapToInlinePointDistance = 0.5f;

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
        if (_snapToPreviousPoint)
        {
            Vector3 prevPoint = FindClosestPoint(point, prevSelecPoints);
            bool inPrevSnapRange = Vector3.Distance(prevPoint, point) < _snapToPreviousPointDistance;
            if (inPrevSnapRange) return prevPoint;
        }

        if (_snapToInlinePoint)
        {
            List<Vector3> inlinePoints = InlineSnapping.GetPoints(point, prevSelecPoints);
            Vector3 inlinePoint = FindClosestPoint(point, inlinePoints);
            bool inInlineSnapRange = Vector3.Distance(inlinePoint, point) < _snapToInlinePointDistance;
            if (inInlineSnapRange) return inlinePoint;
        }

        return point;
    }

    public static Vector3 FindClosestPoint(Vector3 point, List<Vector3> candidatePoints)
    {
        Vector3 closest = point;
        float distance = Mathf.Infinity;

        foreach (Vector3 candidate in candidatePoints)
        {
            float candidateDistance = Vector3.Distance(candidate, point);
            Debug.Log($"Candidate poing: {candidate} --> {candidateDistance}");
            if (candidateDistance < distance)
            {
                distance = candidateDistance;
                closest = candidate;
            }
        }

        return closest;
    }
}