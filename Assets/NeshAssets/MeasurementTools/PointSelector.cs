using System;
using UnityEngine;

public class PointSelector : MonoBehaviour
{
    [SerializeField] private InputListener _inputListener;
    private Camera _mainSceneCamera;
    private const float _maxRayLength = Mathf.Infinity;
    public event Action<Vector3> OnPointSelected;

    private void OnEnable()
    {
        _inputListener.OnClick += TrySelectPoint;
    }
    private void OnDisable()
    {
        _inputListener.OnClick -= TrySelectPoint;
    }
    private void Awake()
    {
        /* 
         * [OPTIMISATION]
         * Get the scene's main camera so we don't have to look it up repeatedly.
         */
        _mainSceneCamera = Camera.main;
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
}
