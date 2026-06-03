using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * MeasurementToolManager
 * 
 * This class does the following:
 *      - Input reading: gets input from the input listener to trigger actions.
 *      - Tool switching: Keep track of the current tool and switch between available tools.
 *      - Point selecting: Getting points from the point selector and passes them to the tools.
 *      - Events: triggers other scripts if event is significant to them.
 */
public class MeasurementToolManager : MonoBehaviour
{
    // Dev mode enables the point identifier.
    [SerializeField] bool _devMode = true;
    // Debug mode creates console logs.
    [SerializeField] bool _debugMode = true;

    [SerializeField] private PointSelector _selector;
    [SerializeField] private InputListener _listener;

    [SerializeField] private MeasuringTape _measuringTape;
    [SerializeField] private Protractor _protractor;
    [SerializeField] private ContinuousMeasurementTool _continuousMeasurementTool;
    [SerializeField] private PointIndentifier _pointIndentifier;

    public event Action<List<Vector3>> OnSelectPoint;
    public event Action<List<Vector3>> OnMeasurementCreated;
    public event Action OnToolSwitch;

    private IMeasurement _currMeasurement = new InvalidMeasurement();
    private List<IMeasurementTool> _tools = new List<IMeasurementTool>();
    private IMeasurementTool _activeTool;
    private int _activeToolIndex = -1;

    private void Awake()
    {
        InitialiseTools();
        ReadInput();
    }
    private void OnDisable() { StopReadingInput(); }

    private bool InitialiseTools()
    {
        if (_tools == null || _measuringTape == null || _protractor == null || _continuousMeasurementTool == null )
            return false;

        _tools.Add(_measuringTape);
        _tools.Add(_protractor);
        _tools.Add(_continuousMeasurementTool);
        
        if (_devMode && _pointIndentifier != null) 
            _tools.Add(_pointIndentifier);

        _activeToolIndex = _tools.IndexOf(_measuringTape);
        SwitchToTool(_activeToolIndex);

        return true;
    }
    private bool SwitchToTool(int toolIndex)
    {
        if (_tools == null) 
            return false;

        if (toolIndex < 0 || toolIndex > _tools.Count - 1) 
            return false;

        _activeToolIndex = toolIndex;
        _activeTool = _tools[toolIndex];

        // Reset points when switching tools.
        ResetAllPoints();

        OnToolSwitch?.Invoke();

        if (_debugMode) 
            DebugMeasurementTools.Log($"Active tool switched --> {_activeTool.Name}", "MeasManager");

        return true;
    }
    private void NextTool()
    {
        if (_tools == null || _activeTool == null) 
            return;

        bool isLastTool = (_activeToolIndex == _tools.Count - 1);
        int toolIndex = (isLastTool) ? 0 : _activeToolIndex + 1;

        SwitchToTool(toolIndex);
    }
    private void PrevTool()
    {
        if (_tools == null || _activeTool == null) 
            return;

        bool isFirstTool = (_activeToolIndex == 0);
        int toolIndex = (isFirstTool) ? _tools.Count - 1 : _activeToolIndex - 1;

        SwitchToTool(toolIndex);
    }

    private void SelectPoint(Vector2 screenPoint)
    {
        if (_selector == null || _activeTool == null) 
            return;

        Vector3 point = _selector.GetPoint(screenPoint, _activeTool.SelectedPoints);

        // return if point was not found
        if (point == null || point == Vector3.zero) 
            return;

        if (_debugMode) 
            DebugMeasurementTools.Log($"Point selected {point}", "MeasManager");

        IMeasurement measurement = _activeTool.TakePoint(point); 
        OnSelectPoint?.Invoke(_activeTool.SelectedPoints);

        HandleMeasurement(measurement);
    }
    private void DeselectLastPoint()
    {
        if (_activeTool == null) 
            return;

        _activeTool.RemoveLastSelectedPoint();
        OnSelectPoint?.Invoke(_activeTool.SelectedPoints);
    }
    private void ResetAllPoints()
    {
        if (_activeTool == null) 
            return;

        HandleMeasurement(new InvalidMeasurement());

        _activeTool.ResetAllSelectedPoints();
        OnSelectPoint?.Invoke(_activeTool.SelectedPoints);
    }
    private void HandleMeasurement(IMeasurement measurement)
    {
        _currMeasurement = measurement;

        if (measurement is InvalidMeasurement)
            return;

        OnMeasurementCreated?.Invoke(((Measurement)_currMeasurement).GetPoints());
    }

    private bool ReadInput()
    {
        if (_listener == null) 
            return false;

        _listener.OnNextTool += NextTool;
        _listener.OnPrevTool += PrevTool;
        _listener.OnSelectPoint += SelectPoint;
        _listener.OnDeselectLastPoint += DeselectLastPoint;
        _listener.OnResetAllPoints += ResetAllPoints;

        return true;
    }
    private bool StopReadingInput()
    {
        if (_listener == null) 
            return false;

        _listener.OnNextTool -= NextTool;
        _listener.OnPrevTool -= PrevTool;
        _listener.OnSelectPoint -= SelectPoint;
        _listener.OnDeselectLastPoint -= DeselectLastPoint;
        _listener.OnResetAllPoints -= ResetAllPoints;

        return true;
    }

    public Sprite GetActiveToolIcon() => _activeTool.ToolIcon;
    public List<Vector3> GetSelectedPoints() => _activeTool.SelectedPoints;
}