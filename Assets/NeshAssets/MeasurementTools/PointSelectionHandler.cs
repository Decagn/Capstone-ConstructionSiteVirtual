using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PointSelectionHandler : MonoBehaviour
{
    [SerializeField] private UserInputHandler _inputHandler;

    private Camera _sceneCamera;

    /*
     * [SYSTEM DESIGN]
     * Max ray length determines how far in the scene can points be selected.
     * Adjust to a distance that makes sense in terms of how far we should be able to measure.
     */
    private const float _maxRayLength = Mathf.Infinity;

    private List<Vector3> _selectedPoints = new List<Vector3>();

    public event Action<Vector3> OnPointSelected;
    //public event Action OnLastPointDeselected;
    //public event Action OnSelectedPointsResetted;

    private void Awake()
    {
        /* 
         * [OPTIMISATION]
         * Get the scene's main camera so we don't have to look it up repeatedly.
         */
        _sceneCamera = Camera.main;

        SubscribeToInputHandler();
    }
    private void OnDisable()
    {
        UnsubscribeFromInputHandler();
    }

    private void TrySelectPoint(Vector2 pointOnScreen)
    {
        Ray ray = _sceneCamera.ScreenPointToRay(pointOnScreen);
        bool surfaceFound = Physics.Raycast(ray, out RaycastHit hit, _maxRayLength);
        if (!surfaceFound) return;

        Vector3 pointOnSurface = hit.point;
        _selectedPoints.Add(pointOnSurface);
        Debug.Log($"PointSelectionHandler: Point selected --> {pointOnSurface}");
        OnPointSelected?.Invoke(pointOnSurface);
    }
    private void DeselectLastPoint()
    {
        if (_selectedPoints.Count <= 0) return;
        _selectedPoints.RemoveAt(_selectedPoints.Count - 1);
        //Debug.Log($"PointSelectionHandler: Last point deselected");
        //OnLastPointDeselected?.Invoke();
    }
    private void ResetSelectedPoints()
    {
        if (_selectedPoints.Count <= 0) return;
        _selectedPoints.Clear();
        //Debug.Log($"PointSelectionHandler: Selected points resetted");
        //OnSelectedPointsResetted?.Invoke();
    }

    //public List<Vector3> GetSelectedPoints() => _selectedPoints;

    private void SubscribeToInputHandler()
    {
        if (_inputHandler != null)
        {
            _inputHandler.OnSelectPoint += TrySelectPoint;
            _inputHandler.OnDeselectLastPoint += DeselectLastPoint;
            _inputHandler.OnResetSelectedPoints += ResetSelectedPoints;
        }
    }
    private void UnsubscribeFromInputHandler()
    {
        if (_inputHandler != null)
        {
            _inputHandler.OnSelectPoint -= TrySelectPoint;
            _inputHandler.OnDeselectLastPoint -= DeselectLastPoint;
            _inputHandler.OnResetSelectedPoints -= ResetSelectedPoints;
        }
    }
}
