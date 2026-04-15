using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    private InputListener _inputListener;
    private PointSelector _pointSelector;

    private Sprite _tapeMeasureIcon;
    private Sprite _protractorIcon;

    private List<IMeasuringTool> _tools = new List<IMeasuringTool>();
    public IMeasuringTool activeTool;

    public void Initialise(InputListener inputListener, PointSelector pointSelector, Sprite tapeMeasureIcon, Sprite protractorIcon)
    {
        _tapeMeasureIcon = tapeMeasureIcon;
        _protractorIcon = protractorIcon;
        InitialiseTools();

        ConnectPointSelector(pointSelector);
        ConnectInputListener(inputListener);
    }

    private void InitialiseTools()
    {
        Protractor protractor = gameObject.AddComponent<Protractor>();
        protractor.Initialise(_protractorIcon);
        _tools.Add(protractor);

        TapeMeasure tapeMeasure = gameObject.AddComponent<TapeMeasure>();
        tapeMeasure.Initialise(_tapeMeasureIcon);
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

    private void ConnectInputListener(InputListener inputListener)
    {
        _inputListener = inputListener;
        _inputListener.OnResetMeasurement += activeTool.ResetSelectedPoints;
        _inputListener.OnDeselectLastPoint += activeTool.RemoveLastSelectedPoint;
    }
}
