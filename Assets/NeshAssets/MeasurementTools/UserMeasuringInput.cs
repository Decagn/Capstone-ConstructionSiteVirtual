using UnityEngine;
using UnityEngine.InputSystem;

public class UserMeasuringInput : MonoBehaviour
{
    [SerializeField] private PointSelector _pointSelector;
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
            _pointSelector.TrySelectPoint(GetScreenPosition());
        }
    }

    private Vector2 GetScreenPosition()
    {
        return Mouse.current.position.ReadValue();
    }
}
