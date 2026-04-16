using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MeasurementToolHandler : MonoBehaviour
{
    [SerializeField] private UserInputHandler _inputHandler;
    [SerializeField] private PointSelectionHandler _pointsHandler;

    [SerializeField] private Sprite _tapeMeasureIcon;
    [SerializeField] private Sprite _protractorIcon;

    private List<IMeasurementTool> _toolBelt = new List<IMeasurementTool>();
    private IMeasurementTool _activeTool;
    int _activeToolIndex = -1;

    public event Action<Vector3> OnPointSelected;
    public event Action<IMeasurementTool> OnSwitchTool;
    public event Action<float, IMeasurementTool> OnMeasurementUpdated;
    public event Action<IMeasurementTool> OnSelectedPointsResettedManually;
    public event Action<IMeasurementTool> OnDeselectLastPoint;

    private List<Vector3> _selectedPoints = new List<Vector3>();
    private bool wasMeasurementUpdated = false;

    private void Awake()
    {
        SubscribeToInputHandler();
        SubscribeToPointHandler();
        InitialiseToolBelt();
    }
    private void OnDisable()
    {
        UnsubscribeFromInputHandler();
        UnsubscribedFromPointHandler();
    }

    private void InitialiseToolBelt()
    {
        TapeMeasure tapeMeasure = gameObject.AddComponent<TapeMeasure>();
        tapeMeasure.Initialise(_tapeMeasureIcon);
        _toolBelt.Add(tapeMeasure);

        Protractor protractor = gameObject.AddComponent<Protractor>();
        protractor.Initialise(_protractorIcon);
        _toolBelt.Add(protractor);

        _activeToolIndex = 0;
        SwitchToTool(_activeToolIndex);
    }
    private void SwitchToTool(int index)
    {
        _activeTool = _toolBelt[index];
        OnSwitchTool?.Invoke(_activeTool);
        _selectedPoints.Clear();
        Debug.Log($"MeasurementToolHandler:: Switched to {_activeTool.ToolName}");
    }
    private void SwitchToNextTool()
    {
        bool isLastTool = (_activeToolIndex >= _toolBelt.Count - 1);
        _activeToolIndex = isLastTool ? 0 : _activeToolIndex + 1;
        SwitchToTool(_activeToolIndex);
    }
    private void SwitchtoPrevTool()
    {
        bool isFirstTool = (_activeToolIndex == 0);
        _activeToolIndex = isFirstTool ? _toolBelt.Count - 1 : _activeToolIndex - 1;
        SwitchToTool(_activeToolIndex);
    }

    private void HandleSelectedPoint(Vector3 point)
    {
        if (wasMeasurementUpdated) ResetSelectedPoints();

        _selectedPoints.Add(point);
        OnPointSelected?.Invoke(point);

        bool MeasurementCreated = _activeTool.CreateMeasurement(_selectedPoints);
        if (!MeasurementCreated)
        {
            wasMeasurementUpdated = false;
            return;
        }

        wasMeasurementUpdated = true;
        float newMeasurement = _activeTool.CurrentMeasurement.value;
        Debug.Log($"MeasurementToolHandler: {_activeTool.ToolName} just measured {newMeasurement}");
        OnMeasurementUpdated?.Invoke(newMeasurement, _activeTool);
    }
    private void DeselectLastPoint()
    {
        if (_selectedPoints.Count < 1) return;
        _selectedPoints.RemoveAt(_selectedPoints.Count - 1);
        wasMeasurementUpdated = false;
        Debug.Log($"MeasurementToolHandler:: Last point deselected");
        OnDeselectLastPoint?.Invoke(_activeTool);
    }
    private void ResetSelectedPoints()
    {
        _selectedPoints.Clear();
        Debug.Log($"MeasurementToolHandler:: Selected points resetted");
        OnSelectedPointsResettedManually?.Invoke(_activeTool);
    }

    
    private void SubscribeToInputHandler()
    {
        if (_inputHandler != null)
        {
            _inputHandler.OnSwitchNextTool += SwitchToNextTool;
            _inputHandler.OnSwitchPrevTool += SwitchtoPrevTool;
            _inputHandler.OnDeselectLastPoint += DeselectLastPoint;
            _inputHandler.OnResetSelectedPoints += ResetSelectedPoints;
        }
    }
    private void UnsubscribeFromInputHandler()
    {
        if (_inputHandler != null)
        {
            _inputHandler.OnSwitchNextTool -= SwitchToNextTool;
            _inputHandler.OnSwitchPrevTool -= SwitchtoPrevTool;
            _inputHandler.OnDeselectLastPoint -= DeselectLastPoint;
            _inputHandler.OnResetSelectedPoints -= ResetSelectedPoints;
        }
    }
    private void SubscribeToPointHandler()
    {
        if (_pointsHandler != null)
        {
            _pointsHandler.OnPointSelected += HandleSelectedPoint;
        }
    }
    private void UnsubscribedFromPointHandler()
    { 
        if (_pointsHandler != null)
        {
            _pointsHandler.OnPointSelected -= HandleSelectedPoint;
        }
    }
}
