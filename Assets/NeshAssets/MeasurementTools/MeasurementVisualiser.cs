using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MeasurementVisualiser : MonoBehaviour
{
    private ToolManager _toolManager;

    private Material _lineMaterial;
    private float _lineThickness = 0.02f;
    private Color _lineColor = Color.green;
    private LineRenderer _lineRenderer;

    private GameObject _pointMarkerPrefab;
    private List<GameObject> _pointMarkers;

    public void Initialise(ToolManager toolManager, GameObject pointMarkerPrefab, Material lineMaterial)
    {
        ConnectToolManager(toolManager);
        _toolManager.activeTool.OnSelectedPointsUpdated += VisualiseMeasurement;

        _pointMarkers = new List<GameObject>();
        SetPointMarkerPrefab(pointMarkerPrefab);

        _lineMaterial = lineMaterial;
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        SetMeasurementLineAppearance();
    }

    private void OnDisable()
    {
        _toolManager.activeTool.OnSelectedPointsUpdated -= VisualiseMeasurement;
    }

    private void DrawLine(List<Vector3> selectedPoints)
    {
        _lineRenderer.positionCount = selectedPoints.Count;
        for (int i = 0;  i < _lineRenderer.positionCount; i++)
        {
            _lineRenderer.SetPosition(i, selectedPoints[i]);
        }
    }

    private void DrawPointMarkers(List<Vector3> selectedPoints)
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

    private void VisualiseMeasurement()
    {
        List<Vector3> selectedPoints = _toolManager.activeTool.SelectedPoints;
        DrawPointMarkers(selectedPoints);
        DrawLine(selectedPoints);
    }

    #region UTILITY METHODS
    private void ConnectToolManager(ToolManager manager)
    {
        _toolManager = manager;
    }

    private void SetPointMarkerPrefab(GameObject pointMarkerPrefab)
    {
        _pointMarkerPrefab = pointMarkerPrefab;
    }

    private void SetMeasurementLineAppearance()
    {
        _lineRenderer.material = _lineMaterial;
        _lineRenderer.startWidth = _lineThickness;
        _lineRenderer.endWidth = _lineThickness;
        _lineRenderer.startColor = _lineColor;
        _lineRenderer.endColor = _lineColor;
    }
    #endregion
}
