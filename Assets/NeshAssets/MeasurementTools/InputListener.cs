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

    public void Initialise(InputAction clickButton)
    {
        SetClickButton(clickButton);
        _clickButton.Enable();
    }

    private void OnDisable()
    {
        _clickButton?.Disable();
    }
    private void Update()
    {
        if (_clickButton.WasPressedThisFrame())
        {
            OnClick?.Invoke(GetClickPosition());
        }
    }
    private Vector2 GetClickPosition()
    {
        return Mouse.current.position.ReadValue();
    }

    public void SetClickButton(InputAction button)
    {
        _clickButton = button;
    }
}