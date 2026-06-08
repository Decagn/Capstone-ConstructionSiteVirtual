using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointSelector : MonoBehaviour
{
    // The player's camera to draw raycasts from.
    [SerializeField] private Camera _playerCam; 

    // How far in the scene points can be selected from.
    [SerializeField] private float _furthestPoint = Mathf.Infinity;

    [SerializeField] private bool _snapping = true;

    // Snapping to points placed in the lesson plan.
    [SerializeField] private LessonMeasurementsManager _lessonSnapManager;
    [SerializeField] private bool _lessonPointSnapping = false;
    [SerializeField] private float _snapToLessonPointDistance = 0.2f;

    // Snapping to points that have previously been placed in the scene.
    [SerializeField] private bool _previousPointSnapping = true;
    [SerializeField] private float _snapToPreviousPointDistance = 0.2f;

    // Snapping to the vertical line or horizontal plane of the other points in the scene.
    [SerializeField] private bool _inlinePointSnapping = false;
    [SerializeField] private float _snapToInlinePointDistance = 0.2f;

    [SerializeField] private MeasurementToolManager _measManager;
    [SerializeField] private InputListener _inputListener;

    bool _platformIsMobile = Application.isMobilePlatform 
        || UnityEngine.Device.Application.isMobilePlatform;

    /*
     * Preview Point
     * 
     * Shows the position of the point on screen before the user selects it.
     * previewPoint is the small point in the center.
     * previewGrid is the larger rotating sphere with a grid texture.
     */
    [SerializeField] private bool previewPoint = true;
    [SerializeField] private float _previewPointSize = 0.03f;
    [SerializeField] private GameObject _previewGridPrefab;
    [SerializeField] private GameObject _previewPointPrefab;

    private float _previewGridSize;
    private float _gridRotationSpeed = 100f;
    private GameObject _previewPoint;
    private GameObject _previewGrid;

    // Dynamically scale the grid to the preview point size.
    private void Awake() { _previewGridSize = _previewPointSize * 4f; } 
    private void Update() { if(previewPoint) ShowPreviewPoint(); }
    private void OnEnable() { _inputListener.OnToggleSnapping += ToggleSnapping; }
    private void OnDisable() { _inputListener.OnToggleSnapping -= ToggleSnapping; }

    private Vector3 TryGetPoint(Vector2 screenPoint)
    {
        Ray ray = _playerCam.ScreenPointToRay(screenPoint);
        Physics.Raycast(ray, out RaycastHit hit, _furthestPoint);
        return hit.point;
    }

    // Searches a list of vectors and returns the closest point.
    public static Vector3 FindClosestPoint(Vector3 point, List<Vector3> candidatePoints)
    {
        Vector3 closestPoint = point;
        float distance = Mathf.Infinity;

        foreach (Vector3 candidate in candidatePoints)
        {
            float candidateDistance = Vector3.Distance(candidate, point);
            if (candidateDistance < distance)
            {
                distance = candidateDistance;
                closestPoint = candidate;
            }
        }

        return closestPoint;
    }
    
    private Vector3 GetPointToSnapTo(Vector3 point, List<Vector3> prevSelecPoints)
    {
        if (_lessonPointSnapping)
        {
            List<Vector3> LessonPoints = _lessonSnapManager.GetLessonMeasurementPoints();
            Vector3 lessonPoint = FindClosestPoint(point, LessonPoints);
            bool inLessonSnappingRange = Vector3.Distance(point, lessonPoint) < _snapToLessonPointDistance;
            if (inLessonSnappingRange) 
                return lessonPoint;
        }

        if (_previousPointSnapping && prevSelecPoints.Count > 0)
        {
            Vector3 prevPoint = FindClosestPoint(point, prevSelecPoints);
            bool inPrevSnapRange = Vector3.Distance(prevPoint, point) < _snapToPreviousPointDistance;
            if (inPrevSnapRange) 
                return prevPoint;
        }

        if (_inlinePointSnapping)
        {
            List<Vector3> inlinePoints = InlineSnapping.GetPoints(point, prevSelecPoints);
            Vector3 inlinePoint = FindClosestPoint(point, inlinePoints);
            bool inInlineSnapRange = Vector3.Distance(inlinePoint, point) < _snapToInlinePointDistance;
            if (inInlineSnapRange) 
                return inlinePoint;
        }

        return point;
    }

    public Vector3 GetPoint(Vector2 screenPoint, List<Vector3> selecPoints)
    {
        Vector3 point = TryGetPoint(screenPoint);

        if (!_snapping) 
            return point;

        Vector3 snapPoint = GetPointToSnapTo(point, selecPoints);
        return snapPoint;
    }

    /*
     * Preview Updates
     * 
     * Change the position and relative size of the preview each frame.
     */
    private void UpdatePreview(Vector3 pointInWorld)
    {
        _previewPoint.transform.position = pointInWorld;
        Scaler.ScaleObject(_playerCam, _previewPoint, _previewPointSize);

        _previewGrid.transform.position = pointInWorld;
        _previewGrid.transform.Rotate(Vector3.up * _gridRotationSpeed * Time.deltaTime);
        Scaler.ScaleObject(_playerCam, _previewGrid, _previewGridSize);
    }

    private void ShowPreviewPoint()
    {
        // On mobile, the screen point is just the middle of the screen.
        Vector2 screenPoint = _platformIsMobile ? new Vector2(Screen.width / 2f, Screen.height / 2f) : Mouse.current.position.ReadValue();

        // Use the same logic as point selecting so preview point would visually snap to snapping points.
        Vector3 pointInWorld = GetPoint(screenPoint, _measManager.GetSelectedPoints());

        if (_previewGrid == null)
            _previewGrid = GameObject.Instantiate(_previewGridPrefab);

        if (_previewPoint == null)
            _previewPoint = GameObject.Instantiate(_previewPointPrefab);

        UpdatePreview(pointInWorld);
    }

    public void ToggleSnapping() { _snapping = !_snapping; }
}