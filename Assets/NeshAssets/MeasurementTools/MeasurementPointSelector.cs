using UnityEngine;
using System;

public class MeasurementPointSelector : MonoBehaviour
{
    private const float _maxRayLength = Mathf.Infinity;

    public event Action<Vector3> OnPointSelected;

    private Camera _mainCamera;

    private void Awake()
    {
        /* 
         * OPTIMISATION
         * Get the scene's main camera so we don't have to look it up repeatedly.
         */
        _mainCamera = Camera.main;
    }

    public void TrySelectPoint(Vector2 positionOnScreen)
    {
        Ray castRay = _mainCamera.ScreenPointToRay(positionOnScreen);
        bool surfaceFound = Physics.Raycast(castRay, out RaycastHit hitPoint, _maxRayLength);

        if (surfaceFound)
        {
            Debug.Log($"Point selected: {hitPoint.point}");
            OnPointSelected?.Invoke(hitPoint.point);
        }
    }
}