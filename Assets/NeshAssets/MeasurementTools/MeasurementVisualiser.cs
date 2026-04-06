using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MeasurementVisualiser : MonoBehaviour
{
    [SerializeField] private Material _lineMaterial;
    [SerializeField] private float _lineThickness = 0.02f;
    [SerializeField] private Color _lineColor = Color.green;
    private LineRenderer _lineRenderer;

    [SerializeField] private GameObject _pointMarkerPrefab;
    private List<GameObject> _pointMarkers;

    private void Awake()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.material = _lineMaterial;
        _lineRenderer.startWidth = _lineThickness;
        _lineRenderer.endWidth = _lineThickness;
        _lineRenderer.startColor = _lineColor;
        _lineRenderer.endColor = _lineColor;
    }

    public void DrawLine(List<Vector3> selectedPoints)
    {
        _lineRenderer.positionCount = selectedPoints.Count;
        for (int i = 0;  i < _lineRenderer.positionCount; i++)
        {
            _lineRenderer.SetPosition(i, selectedPoints[i]);
        }
    }
}
