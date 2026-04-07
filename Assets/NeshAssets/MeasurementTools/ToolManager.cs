using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    private PointSelector _pointSelector;

    private List<IMeasuringTool> _tools = new List<IMeasuringTool>();
    public IMeasuringTool activeTool;

    public void Initialise(PointSelector pointSelector, Sprite tapeMeasureIcon)
    {
        ConnectPointSelector(pointSelector);
        InitialiseTools(tapeMeasureIcon);
    }

    private void InitialiseTools(Sprite tapeMeasureIcon)
    {
        TapeMeasure tapeMeasure = gameObject.AddComponent<TapeMeasure>();
        tapeMeasure.Initialise(tapeMeasureIcon);
        _tools.Add(tapeMeasure);

        activeTool = _tools[0];
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
        activeTool?.HandleSelectedPoint(selectedPoint);
    }

    private void ConnectPointSelector(PointSelector pointSelector)
    {
        _pointSelector = pointSelector;
        _pointSelector.OnPointSelected += HandleSelectedPoint;
    }
}
