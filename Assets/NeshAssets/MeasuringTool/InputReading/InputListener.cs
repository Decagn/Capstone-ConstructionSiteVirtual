using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputListener : MonoBehaviour
{
    public event Action<Vector2> OnSelectPoint;
    public event Action OnDeselectLastPoint;
    public event Action OnResetAllPoints;
    public event Action OnNextTool;
    public event Action OnPrevTool;

    [SerializeField] private InputAction _selectPoint;
    [SerializeField] private InputAction _deselectLastPoint;
    [SerializeField] private InputAction _resetAllPoints;
    [SerializeField] private InputAction _nextTool;
    [SerializeField] private InputAction _prevTool;

    private void OnEnable()
    {
        _selectPoint.Enable();
        _deselectLastPoint.Enable();
        _resetAllPoints.Enable();
        _nextTool.Enable();
        _prevTool.Enable();
    }

    private void OnDisable()
    {
        _selectPoint.Disable();
        _deselectLastPoint.Disable();
        _resetAllPoints.Disable();
        _nextTool.Disable();
        _prevTool.Disable();
    }

    private void Awake()
    {
        AddAllDefaultBindings();
    }

    private void Update()
    {
        if (_selectPoint.WasPressedThisFrame()) OnSelectPoint?.Invoke(GetClickPostion());
        else if (_deselectLastPoint.WasPressedThisFrame()) OnDeselectLastPoint?.Invoke();
        else if (_resetAllPoints.WasPressedThisFrame()) OnResetAllPoints?.Invoke();
        else if (_nextTool.WasPressedThisFrame()) OnNextTool?.Invoke();
        else if (_prevTool.WasPressedThisFrame()) OnPrevTool?.Invoke();
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
        AddDefaultBinding(_resetAllPoints, "<Keyboard>/r");
        AddDefaultBinding(_nextTool, "<Mouse>/scroll/up");
        AddDefaultBinding(_prevTool, "<Mouse>/scroll/down");
    }
}
