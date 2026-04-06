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
    [SerializeField] private InputAction _click;
    private void OnEnable()
    {
        _click.Enable();
    }
    private void OnDisable()
    {
        _click.Disable();
    }
    private void Update()
    {
        if (_click.WasPressedThisFrame())
        {
            OnClick?.Invoke(GetClickPosition());
        }
    }
    private Vector2 GetClickPosition()
    {
        return Mouse.current.position.ReadValue();
    }
}