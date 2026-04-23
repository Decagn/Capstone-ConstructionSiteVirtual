using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MeasurementToolHandler : MonoBehaviour
{
    [SerializeField] private UserInputHandler _inputHandler;
    [SerializeField] private TapeMeasure _tapeMeasure;
    [SerializeField] private Protractor _protractor;
    [SerializeField] private MultiAngleRuler _multiAngleRuler;

    [SerializeField] private bool _enableSnapping = true;
    [SerializeField] private float _snapDistance = 0.2f;

    private List<IMeasurementTool> _toolBelt = new List<IMeasurementTool>();
    private IMeasurementTool _activeTool;
    int _activeToolIndex = -1;

    public event Action<IMeasurementTool, Vector3> OnPointSelected;
    public event Action<IMeasurementTool> OnSwitchTool;
    public event Action<IMeasurementTool, float> OnMeasurementUpdated;
    public event Action<IMeasurementTool> OnSelectedPointsResettedManually;
    public event Action<IMeasurementTool> OnDeselectLastPoint;
    public event Action OnToolBeltInitialised;

    private List<Vector3> _selectedPoints = new List<Vector3>();
    private bool wasMeasurementUpdated = false;

    private void Awake()
    {
        SubscribeToInputHandler();
        InitialiseToolBelt();
    }
    private void OnDisable()
    {
        UnsubscribeFromInputHandler();
    }

    private void InitialiseToolBelt()
    {
        _toolBelt.Add(_tapeMeasure);
        _toolBelt.Add(_protractor);
        _toolBelt.Add(_multiAngleRuler);
        _activeToolIndex = 0;
        OnToolBeltInitialised?.Invoke();
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

    private void TrySelectPoint(Vector2 pointOnScreen)
    {
        Vector3 point = PointSelector.TrySelectPoint(pointOnScreen, _selectedPoints, _enableSnapping, _snapDistance);

        bool invalidPoint = (point == null || point == Vector3.zero);
        if (invalidPoint) return;

        HandleSelectedPoint(point);
    }
    private void HandleSelectedPoint(Vector3 point)
    {
        if (wasMeasurementUpdated && _activeTool is not MultiAngleRuler) ResetSelectedPoints();

        _selectedPoints.Add(point);
        OnPointSelected?.Invoke(_activeTool, point);

        bool MeasurementCreated = _activeTool.CreateMeasurement(_selectedPoints);
        if (!MeasurementCreated)
        {
            wasMeasurementUpdated = false;
            return;
        }

        wasMeasurementUpdated = true;
        float newMeasurement = _activeTool.CurrentMeasurement.value;
        Debug.Log($"MeasurementToolHandler: {_activeTool.ToolName} just measured {newMeasurement}");
        OnMeasurementUpdated?.Invoke(_activeTool, newMeasurement);
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
            _inputHandler.OnSelectPoint += TrySelectPoint;
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
            _inputHandler.OnSelectPoint -= TrySelectPoint;
            _inputHandler.OnSwitchNextTool -= SwitchToNextTool;
            _inputHandler.OnSwitchPrevTool -= SwitchtoPrevTool;
            _inputHandler.OnDeselectLastPoint -= DeselectLastPoint;
            _inputHandler.OnResetSelectedPoints -= ResetSelectedPoints;
        }
    }
}
