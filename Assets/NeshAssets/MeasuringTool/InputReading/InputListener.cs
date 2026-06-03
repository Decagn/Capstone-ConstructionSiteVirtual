using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputListener : MonoBehaviour
{
    /*
     * Public Events
     * 
     * Other scripts can subscribe to these events to call their own functions.
     * This reduces tight coupling between the scripts.
     */
    public event Action<Vector2> OnSelectPoint; // Passes the position of the mouse cursor.
    public event Action OnDeselectLastPoint;
    public event Action OnResetAllPoints;
    public event Action OnNextTool;
    public event Action OnPrevTool;
    public event Action OnToggleSnapping;

    private InputAction _selectPoint;
    private InputAction _deselectLastPoint;
    private InputAction _resetAllPoints;
    private InputAction _nextTool;
    private InputAction _prevTool;
    private InputAction _toggleSnapping;

    private bool _platformIsMobile = Application.isMobilePlatform 
        || UnityEngine.Device.Application.isMobilePlatform;

    private void OnEnable() { EnableAllActions(); }
    private void OnDisable() { DisableAllActions(); }
    private void Awake()
    {
        SetupActions();
        if (_platformIsMobile) 
            DebugMeasurementTools.Log("Mobile mode", "InputListener");
        else 
            DebugMeasurementTools.Log("Desktop mode", "InputListener");
    }
    private void Update()
    {
        if (_platformIsMobile) 
            return;
        HandleDesktopInput();
    }

    private void EnableAllActions()
    {
        _selectPoint?.Enable();
        _deselectLastPoint?.Enable();
        _resetAllPoints?.Enable();
        _nextTool?.Enable();
        _prevTool?.Enable();
        _toggleSnapping?.Enable();
    }
    private void DisableAllActions()
    {
        _selectPoint?.Disable();
        _deselectLastPoint?.Disable();
        _resetAllPoints?.Disable();
        _nextTool?.Disable();
        _prevTool?.Disable();
        _toggleSnapping?.Disable();
    }
    private void SetupActions()
    {
        _selectPoint = new InputAction("SelectPoint");
        _deselectLastPoint = new InputAction("DeselectLastPoint");
        _resetAllPoints = new InputAction("ResetAllPoints");
        _nextTool = new InputAction("NextTool");
        _prevTool = new InputAction("PrevTool");
        _toggleSnapping = new InputAction("ToggleSnapping");

        if (!_platformIsMobile)
            BindDesktopKeys();

        EnableAllActions();
    }

    private void HandleDesktopInput()
    {
        if (_selectPoint.WasPressedThisFrame()) 
            OnSelectPoint?.Invoke(GetClickPostion());

        else if (_deselectLastPoint.WasPressedThisFrame()) 
            OnDeselectLastPoint?.Invoke();

        else if (_resetAllPoints.WasPressedThisFrame()) 
            OnResetAllPoints?.Invoke();

        else if (_nextTool.WasPressedThisFrame()) 
            OnNextTool?.Invoke();

        else if (_prevTool.WasPressedThisFrame()) 
            OnPrevTool?.Invoke();

        else if (_toggleSnapping.WasPressedThisFrame())
            OnToggleSnapping?.Invoke();
    }
    private Vector2 GetClickPostion() => Mouse.current.position.ReadValue();
    private void AddBinding(InputAction action, string binding)
    {
        if (action.bindings.Count == 0) action.AddBinding(binding);
    }
    private void BindDesktopKeys()
    {
        AddBinding(_selectPoint, "<Mouse>/leftButton");
        AddBinding(_deselectLastPoint, "<Keyboard>/x");
        AddBinding(_resetAllPoints, "<Keyboard>/r");
        AddBinding(_nextTool, "<Mouse>/scroll/up");
        AddBinding(_prevTool, "<Mouse>/scroll/down");
        AddBinding(_toggleSnapping, "<Keyboard>/z");
    }

    /*
     * Public Event Calling
     * 
     * These functions allow buttons in the editor to call the events.
     * Used on mobile where controls are bound to on screen buttons.
     */
    public void SelectPoint() { OnSelectPoint?.Invoke(new Vector2(Screen.width / 2f, Screen.height / 2f));}
    public void DeselectPoint() { OnDeselectLastPoint?.Invoke(); }
    public void ResetPoints() { OnResetAllPoints?.Invoke(); }
    public void SwitchNextTool() { OnNextTool?.Invoke(); }
    public void SwitchPrevTool() { OnPrevTool?.Invoke(); }
    public void ToggleSnapping() { OnToggleSnapping?.Invoke(); }
}
