using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    [SerializeField] private PointSelector _pointSelector;
    [SerializeField] private MeasurementVisualiser _visualiser;

    private List<IMeasuringTool> _tools = new List<IMeasuringTool>();
    private IMeasuringTool _activeTool;

    private void Awake()
    {
        TapeMeasure tapeMeasure = gameObject.AddComponent<TapeMeasure>();
        _tools.Add(tapeMeasure);
        _activeTool = _tools[0];
        Debug.Log($"Tool Manage: tool selected --> {_tools[0].ToolName}");
    }

    private void OnEnable()
    {
        _pointSelector.OnPointSelected += HandleSelectedPoint;
    }

    private void OnDisable()
    {
        _pointSelector.OnPointSelected -= HandleSelectedPoint;
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
            _visualiser.VisualiseMeasurement(_activeTool.SelectedPoints);
        }
    }
}
