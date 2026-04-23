using NUnit.Framework;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Visualiser : MonoBehaviour
{
    [SerializeField] MeasurementToolHandler _toolHandler;

    [SerializeField] PointMarkers _markers;
    [SerializeField] Lines _lines;
    [SerializeField] Arcs _arcs;

    private List<Vector3> _selectedPoints = new List<Vector3> ();

    private void Awake()
    {
        SubscribeToToolHandler();
    }
    private void OnDisable()
    {
        UnsubscribeFromToolHandler();
    }

    private void HandleSelectedPoint(IMeasurementTool activeTool, Vector3 point)
    {
        _selectedPoints.Add(point);
        UpdateVisuals(activeTool);
    }
    private void ResetSelectedPoints(IMeasurementTool activeTool)
    {
        _selectedPoints.Clear();
        UpdateVisuals(activeTool);
    }
    private void DeselectLastPoint(IMeasurementTool activeTool)
    {
        if (_selectedPoints.Count < 1) return;
        _selectedPoints.RemoveAt(_selectedPoints.Count - 1);
        UpdateVisuals(activeTool);
    }
    private void UpdateVisuals(IMeasurementTool activeTool)
    {
        _markers.CreateMarkers(_selectedPoints);
        _lines.CreateLines(_selectedPoints, activeTool);
        _arcs.CreateArcs(_selectedPoints);
    }

    private void SubscribeToToolHandler()
    {
        _toolHandler.OnPointSelected += HandleSelectedPoint;
        _toolHandler.OnSelectedPointsResettedManually += ResetSelectedPoints;
        _toolHandler.OnDeselectLastPoint += DeselectLastPoint;
        _toolHandler.OnSwitchTool += ResetSelectedPoints;
    }
    private void UnsubscribeFromToolHandler()
    {
        _toolHandler.OnPointSelected -= HandleSelectedPoint;
        _toolHandler.OnSelectedPointsResettedManually -= ResetSelectedPoints;
        _toolHandler.OnDeselectLastPoint -= DeselectLastPoint;
        _toolHandler.OnSwitchTool += ResetSelectedPoints;
    }
}
