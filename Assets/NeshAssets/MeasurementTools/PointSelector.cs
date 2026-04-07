using System;
using UnityEngine;

public class PointSelector : MonoBehaviour
{
    private InputListener _inputListener;
    private Camera _mainSceneCamera;
    private const float _maxRayLength = Mathf.Infinity;
    public event Action<Vector3> OnPointSelected;

    public void Initialise(InputListener inputListener)
    {
        /* 
         * [OPTIMISATION]
         * Get the scene's main camera so we don't have to look it up repeatedly.
         */
        _mainSceneCamera = Camera.main;
        ConnectInputListener(inputListener);
    }    

    private void OnDisable()
    {
        if (_inputListener != null)
        {
            _inputListener.OnClick -= TrySelectPoint;
        }
    }

    public void TrySelectPoint(Vector2 pointOnScreen)
    {
        Ray ray = _mainSceneCamera.ScreenPointToRay(pointOnScreen);
        bool surfaceFound = Physics.Raycast(ray, out RaycastHit hit, _maxRayLength);
        if (surfaceFound)
        {
            Vector3 pointOnSurface = hit.point;
            Debug.Log($"PointSelector: Point selected --> {pointOnSurface}");
            OnPointSelected?.Invoke(pointOnSurface);
        }
    }

    private void ConnectInputListener (InputListener listener)
    {
        _inputListener = listener;
        _inputListener.OnClick += TrySelectPoint;
    }
}
