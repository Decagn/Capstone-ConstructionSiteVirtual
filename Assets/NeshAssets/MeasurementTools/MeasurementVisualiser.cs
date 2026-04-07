using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MeasurementVisualiser : MonoBehaviour
{
    private float _lineThickness = 0.02f;
    private Color _lineColor = Color.green;
    private LineRenderer _lineRenderer;

    private GameObject _pointMarkerPrefab;
    private List<GameObject> _pointMarkers;

    public void Initialise(GameObject pointMarkerPrefab)
    {
        _pointMarkers = new List<GameObject>();
        SetPointMarkerPrefab(pointMarkerPrefab);

        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        SetMeasurementLineAppearance();
    }

    public void DrawLine(List<Vector3> selectedPoints)
    {
        _lineRenderer.positionCount = selectedPoints.Count;
        for (int i = 0;  i < _lineRenderer.positionCount; i++)
        {
            _lineRenderer.SetPosition(i, selectedPoints[i]);
        }
    }

    public void DrawPointMarkers(List<Vector3> selectedPoints)
    {
        foreach (GameObject pointMarker in _pointMarkers)
        {
           Destroy(pointMarker);
        }
        _pointMarkers.Clear();
        foreach (Vector3 point in selectedPoints)
        {
            GameObject pointMarker = Instantiate(_pointMarkerPrefab);
            pointMarker.transform.position = point;
            _pointMarkers.Add(pointMarker);
        }
    }

    public void VisualiseMeasurement(List<Vector3> selectedPoints)
    {
        DrawPointMarkers(selectedPoints);
        DrawLine(selectedPoints);
    }
    
    private void SetPointMarkerPrefab(GameObject pointMarkerPrefab)
    {
        _pointMarkerPrefab = pointMarkerPrefab;
    }

    private void SetMeasurementLineAppearance()
    {
        _lineRenderer.startWidth = _lineThickness;
        _lineRenderer.endWidth = _lineThickness;
        _lineRenderer.startColor = _lineColor;
        _lineRenderer.endColor = _lineColor;
    }
}
