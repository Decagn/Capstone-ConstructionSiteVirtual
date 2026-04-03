using UnityEngine;
using UnityEngine.InputSystem;

public class MeasurementToolInput : MonoBehaviour
{
    [SerializeField] private MeasurementPointSelector _pointSelector;
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
