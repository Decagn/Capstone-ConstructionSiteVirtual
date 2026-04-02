using UnityEngine;
using System;

public class PointSelector : MonoBehaviour
{
    public event Action<Vector3> OnPointSelected;
    private const float _maxDistanceToSurface = Mathf.Infinity;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    public void TrySelectPoint(Vector2 positionOnScreen)
    {
        Ray ray = _camera.ScreenPointToRay(positionOnScreen);
        if (Physics.Raycast(ray, out RaycastHit hit, _maxDistanceToSurface))
        {
            Debug.Log($"Point selected: {hit.point}");
            OnPointSelected?.Invoke(hit.point);
        }
    }
}