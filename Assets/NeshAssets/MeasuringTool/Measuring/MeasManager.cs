using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeasManager : MonoBehaviour
{
    [SerializeField] bool _lessonCreationMode = true;
    [SerializeField] bool _debugMode = true;

    #region Unity Lifecycle Events
    private void Awake()
    {
        initTools();
        ReadInput();
    }
    private void OnDisable()
    {
        StopReadingInput();
    }
    #endregion

    #region Tool Switching
    [SerializeField] private TapeMeasure _tapeMeasure;
    [SerializeField] private Protractor _protractor;
    [SerializeField] private MultiAngle _multiTool;
    [SerializeField] private POILaser _laser;

    private List<IMeasTool> _tools = new List<IMeasTool>();
    private IMeasTool _activeTool;
    private int _activeToolIdx = -1;
    private List<Vector3> empty = new List<Vector3>();

    public event Action OnToolSwitch;

    private bool initTools()
    {
        if (_tools == null || 
            _tapeMeasure == null ||
            _protractor == null ||
            _multiTool == null )
        { return false; }

        _tools.Add(_tapeMeasure);
        _tools.Add(_protractor);
        _tools.Add(_multiTool);
        
        if (_lessonCreationMode && _laser != null) _tools.Add(_laser);

        _activeToolIdx = _tools.IndexOf(_tapeMeasure);
        SwitchToolIdx(_activeToolIdx);
        return true;
    }
    private void NextTool()
    {
        if (_tools == null || _activeTool == null) return;

        
        bool lastTool = (_activeToolIdx == _tools.Count - 1);
        int toolIndex = (lastTool) ? 0 : _activeToolIdx + 1;
        SwitchToolIdx(toolIndex);
    }
    private void PrevTool()
    {
        if (_tools == null || _activeTool == null) return;

        bool firstTool = (_activeToolIdx == 0);
        int toolIndex = (firstTool) ? _tools.Count - 1 : _activeToolIdx - 1;
        SwitchToolIdx(toolIndex);
    }
    private bool SwitchToolIdx(int toolIdx)
    {
        if (_tools == null) return false;
        if (toolIdx < 0 || toolIdx > _tools.Count - 1) return false;

        _activeToolIdx = toolIdx;
        _activeTool = _tools[toolIdx];
        ResetAllPoints();
        OnToolSwitch?.Invoke();

        if (_debugMode) MeasDebug.Log($"Active tool switched --> {_activeTool.Name}", "MeasManager");
        return true;
    }
    #endregion

    #region Point Selecting
    [SerializeField] private PointSelector _selector;

    public event Action<List<Vector3>> OnSelecPoint;

    private void SelectPoint(Vector2 screenPoint)
    {
        if (_selector == null || _activeTool == null) return;

        Vector3 point = _selector.GetPoint(screenPoint, _activeTool.GetPoints());
        if (point == null || point == Vector3.zero) return;
        if (_debugMode) MeasDebug.Log($"Point selected {point}", "MeasManager");

        IMeas meas = _activeTool.TakePoint(point); 
        OnSelecPoint?.Invoke(_activeTool.GetPoints());
        HandleMeas(meas);
    }
    private void DeselectLastPoint()
    {
        if (_activeTool == null) return;

        _activeTool.RemoveLastPoint();
        OnSelecPoint?.Invoke(_activeTool.GetPoints());
    }
    private void ResetAllPoints()
    {
        if (_activeTool == null) return;

        HandleMeas(new InvalidMeas());
        _activeTool.ResetAllPoints();
        OnSelecPoint?.Invoke(_activeTool.GetPoints());
    }
    #endregion

    #region Measurement Handling
    private MeasLogBook _logBook = new MeasLogBook();
    private IMeas _currMeas = new InvalidMeas();

    public event Action<List<Vector3>> OnMeasCreated;

    private void HandleMeas(IMeas meas)
    {
        _currMeas = meas;

        if (meas is InvalidMeas)
        {
            OnMeasCreated?.Invoke(empty);
            return;
        }

        OnMeasCreated?.Invoke(((Meas)_currMeas).GetPoints());
        _logBook.Add((Meas)meas);
    }
    #endregion

    #region Input Reading
    [SerializeField] private InputListener _listener;

    private bool ReadInput()
    {
        if (_listener == null) return false;

        _listener.OnNextTool += NextTool;
        _listener.OnPrevTool += PrevTool;

        _listener.OnSelectPoint += SelectPoint;
        _listener.OnDeselectLastPoint += DeselectLastPoint;
        _listener.OnResetAllPoints += ResetAllPoints;
        return true;
    }
    private bool StopReadingInput()
    {
        if (_listener == null) return false;

        _listener.OnNextTool -= NextTool;
        _listener.OnPrevTool -= PrevTool;

        _listener.OnSelectPoint -= SelectPoint;
        _listener.OnDeselectLastPoint -= DeselectLastPoint;
        _listener.OnResetAllPoints -= ResetAllPoints;
        return true;
    }
    #endregion

    #region Getters
    public Sprite GetActiveToolIcon() => _activeTool.ToolIcon;
    public Meas GetCurrMeas() => (Meas)_currMeas;
    #endregion
}