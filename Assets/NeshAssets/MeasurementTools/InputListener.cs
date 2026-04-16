using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputListener : MonoBehaviour
{
    /*
     * SYSTEM DESIGN
     * Clicking delegates to functions that take in the mouse's position on screen.
     */
    public event Action<Vector2> OnClick;
    private InputAction _clickButton;

    public event Action OnDeselectLastPoint;
    private InputAction _deselectButton;

    public event Action OnResetMeasurement;
    private InputAction _resetButton;

    public void Initialise(InputAction clickButton, InputAction deselectButton, InputAction resetButton)
    {
        SetClickButton(clickButton);
        _clickButton.Enable();

        SetDeselectButton(deselectButton);
        _deselectButton.Enable();

        SetResetButton(resetButton);
        _resetButton.Enable();
    }

    private void OnDisable()
    {
        _clickButton?.Disable();
        _deselectButton?.Disable();
        _resetButton?.Disable();
    }
    private void Update()
    {
        if (_clickButton.WasPressedThisFrame())
        {
            OnClick?.Invoke(GetClickPosition());
        }
        else if (_deselectButton.WasPressedThisFrame())
        {
            OnDeselectLastPoint?.Invoke();
        }
        else if (_resetButton.WasPressedThisFrame())
        {
            OnResetMeasurement?.Invoke();
        }
    }
    private Vector2 GetClickPosition()
    {
        return Mouse.current.position.ReadValue();
    }

    private void SetClickButton(InputAction button)
    {
        _clickButton = button;
    }

    private void SetResetButton(InputAction button)
    {
        _resetButton = button;
    }

    private void SetDeselectButton(InputAction button)
    {
        _deselectButton = button;
    }
}