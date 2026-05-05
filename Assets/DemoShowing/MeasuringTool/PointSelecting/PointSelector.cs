using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class PointSelector : MonoBehaviour
{
    [SerializeField] private float _furthestPoint = Mathf.Infinity;

    [SerializeField] private bool _snapping = true;

    [SerializeField] private bool _snapToPreviousPoint = true;
    [SerializeField] private float _snapToPreviousPointDistance = 0.5f;

    [SerializeField] private bool _snapToInlinePoint = true;
    [SerializeField] private float _snapToInlinePointDistance = 0.5f;

    [SerializeField] private MeasManager _measManager;
    [SerializeField] private bool previewPoint = true;
    [SerializeField] private GameObject _previewGridPrefab;
    [SerializeField] private GameObject _previewPointPrefab;
    [SerializeField] private float _previewPointSize = 0.03f;
    private GameObject _previewGrid;
    private float _previewGridSize;
    private float _gridRotationSpeed = 100f;
    private GameObject _previewPoint;
    
    private void Awake()
    {
        _previewGridSize = _previewPointSize * 4f;
    }
    private void Update()
    {
        ShowPointOnScreen();
    }
    private void ShowPointOnScreen()
    {
        Vector2 screenPoint = Mouse.current.position.ReadValue();
        Vector3 pointInWorld = GetPoint(screenPoint, _measManager.GetSelectedPoints());

        if (_previewGrid == null)
        {
            GameObject previewGrid = GameObject.Instantiate(_previewGridPrefab);
            previewGrid.transform.position = pointInWorld;
            previewGrid.transform.localScale = new Vector3(_previewGridSize, _previewGridSize, _previewGridSize);
            _previewGrid = previewGrid;
        }
        else
        {
            _previewGrid.transform.position = pointInWorld;
            _previewGrid.transform.Rotate(Vector3.up * _gridRotationSpeed * Time.deltaTime); ;
        }

        if (_previewPoint == null)
        {
            GameObject previewPoint = GameObject.Instantiate(_previewPointPrefab);
            previewPoint.transform.position = pointInWorld;
            previewPoint.transform.localScale = new Vector3(_previewPointSize, _previewPointSize, _previewPointSize);
            _previewPoint = previewPoint;
        }
        else
        {
            _previewPoint.transform.position = pointInWorld;
        }
    }

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
            //Debug.Log($"Candidate point: {candidate} --> {candidateDistance}");
            if (candidateDistance < distance)
            {
                distance = candidateDistance;
                closest = candidate;
            }
        }

        return closest;
    }
}