using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UserInputHandler : MonoBehaviour
{
    public event Action<Vector2> OnSelectPoint;
    public event Action OnDeselectLastPoint;
    public event Action OnResetSelectedPoints;
    public event Action OnSwitchNextTool;
    public event Action OnSwitchPrevTool;
    
    [SerializeField] private InputAction _selectPoint;
    [SerializeField] private InputAction _deselectLastPoint;
    [SerializeField] private InputAction _resetSelectedPoints;
    [SerializeField] private InputAction _switchNextTool;
    [SerializeField] private InputAction _switchPrevTool;

    private void OnEnable()
    {
        _selectPoint.Enable();
        _deselectLastPoint.Enable();
        _resetSelectedPoints.Enable();
        _switchNextTool.Enable();
        _switchPrevTool.Enable();
    }

    private void OnDisable()
    {
        _selectPoint.Disable();
        _deselectLastPoint.Disable();
        _resetSelectedPoints.Disable();
        _switchNextTool.Disable();
        _switchPrevTool.Disable();
    }

    private void Awake()
    {
        AddAllDefaultBindings();
    }

    private void Update()
    {
        if (_selectPoint.WasPressedThisFrame()) OnSelectPoint?.Invoke(GetClickPostion());
        else if (_deselectLastPoint.WasPressedThisFrame()) OnDeselectLastPoint?.Invoke();
        else if ( _resetSelectedPoints.WasPressedThisFrame()) OnResetSelectedPoints?.Invoke();
        else if (_switchNextTool.WasPressedThisFrame()) OnSwitchNextTool?.Invoke();
        else if (_switchPrevTool.WasPressedThisFrame()) OnSwitchPrevTool?.Invoke();
    }

    private Vector2 GetClickPostion() => Mouse.current.position.ReadValue();

    private void AddDefaultBinding(InputAction action, string binding)
    {
        if (action.bindings.Count == 0) action.AddBinding(binding);
    }
    private void AddAllDefaultBindings()
    {
        AddDefaultBinding(_selectPoint, "<Mouse>/leftButton");
        AddDefaultBinding(_deselectLastPoint, "<Mouse>/rightButton");
        AddDefaultBinding(_resetSelectedPoints, "<Keyboard>/r");
        AddDefaultBinding(_switchNextTool, "<Mouse>/scroll/up");
        AddDefaultBinding(_switchPrevTool, "<Mouse>/scroll/down");
    }
}
