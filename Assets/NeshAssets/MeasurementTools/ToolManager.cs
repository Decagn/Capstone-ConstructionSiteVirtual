using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    private PointSelector _pointSelector;
    private MeasurementVisualiser _measurementVisualiser;

    private List<IMeasuringTool> _tools = new List<IMeasuringTool>();
    private IMeasuringTool _activeTool;


    public void Initialise(PointSelector pointSelector, MeasurementVisualiser measurementVisualiser)
    {
        ConnectPointSelector(pointSelector);
        ConnectMeasurementVisualiser(measurementVisualiser);
        InitialiseTools();
    }

    private void InitialiseTools()
    {
        TapeMeasure tapeMeasure = gameObject.AddComponent<TapeMeasure>();
        _tools.Add(tapeMeasure);
        _activeTool = _tools[0];
        Debug.Log($"Tool Manage: tool selected --> {_tools[0].ToolName}");
    }

    private void OnDisable()
    {
        if (_pointSelector != null)
        {
            _pointSelector.OnPointSelected -= HandleSelectedPoint;
        }
    }

    private void HandleSelectedPoint(Vector3 selectedPoint)
    {
        _activeTool?.HandleSelectedPoint(selectedPoint);
        VisualiseMeasurement();
    }

    private void VisualiseMeasurement()
    {
        if (_activeTool != null)
        {
            _measurementVisualiser.VisualiseMeasurement(_activeTool.SelectedPoints);
        }
    }

    private void ConnectPointSelector(PointSelector pointSelector)
    {
        _pointSelector = pointSelector;
        _pointSelector.OnPointSelected += HandleSelectedPoint;
    }

    private void ConnectMeasurementVisualiser(MeasurementVisualiser measurementVisualiser)
    {
        _measurementVisualiser = measurementVisualiser;
    }
}
