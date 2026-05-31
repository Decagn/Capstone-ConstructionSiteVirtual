using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MeasVisualiser : MonoBehaviour
{
    [SerializeField] MeasManager _manager;
    [SerializeField] Camera _camera;

    #region Unity Lifecycle Events
    private void Awake()
    {
        ReadPointUpdates();
        ReadLineUpdates();
        ReadArcUpdates();
    }
    private void LateUpdate()
    {
        UpdatePopups();
        UpdateLines();
        UpdateArcs();
        UpdateMarkers();
    }
    private void OnDisable()
    {
        StopPointUpdates();
        StopLineUpdates();
        StopArcUpdates();
    }
    #endregion

    #region Point Markers
    [Header("Point Markers")]
    [SerializeField] GameObject _markerPrefab;
    [SerializeField] GameObject _markerNewPrefab;
    [SerializeField] float _markerSize = 0.075f;
    private List<GameObject> _markers = new List<GameObject>();
    private List<GameObject> _markersNew = new List<GameObject>();

    private void CreateMarkers(List<Vector3> points)
    {
        foreach (GameObject marker in _markers) Destroy(marker);
        _markers.Clear();

        foreach (Vector3 point in points) _markers.Add(CreateMarker(point, _markerPrefab, _markerSize));
    }
    private void CreateNewMarkers(List<Vector3> points)
    {
        foreach (GameObject marker in _markersNew) Destroy(marker);
        _markersNew.Clear();

        foreach (Vector3 point in points) _markersNew.Add(CreateMarker(point, _markerNewPrefab, _markerSize * 0.99f));
    }
    private GameObject CreateMarker(Vector3 point, GameObject markerPrefab, float size)
    {
        GameObject marker = Instantiate(markerPrefab);
        marker.transform.SetParent(transform);
        marker.transform.position = point;
        marker.transform.localScale = new Vector3(size, size, size);
        return marker;
    }
    private void UpdateMarkers()
    {
        foreach (GameObject marker in _markers)
        {
            if (marker != null)
            {
                Scaler.ScaleObject(_camera, marker, _markerSize);
            }
        }
        foreach (GameObject marker in _markersNew)
        {
            if (marker != null)
            {
                Scaler.ScaleObject(_camera, marker, _markerSize);
            }
        }
    }
    private void ReadPointUpdates()
    {
        _manager.OnSelecPoint += CreateNewMarkers;
        _manager.OnMeasCreated += CreateMarkers;
    }
    private void StopPointUpdates()
    {
        _manager.OnSelecPoint -= CreateNewMarkers;
        _manager.OnMeasCreated -= CreateMarkers;
    }
    #endregion

    #region Lines
    [Header("Lines")]
    [SerializeField] Material _lineMaterial;
    [SerializeField] float _lineWidth = 0.02f;
    private List<GameObject> _lines = new List<GameObject>();
    private List<GameObject> _linePopups = new List<GameObject>();

    public void CreateLines(List<Vector3> points)
    {
        foreach (GameObject line in _lines) Destroy(line);
        _lines.Clear();
        ClearLinePopups();

        if (points.Count < 2) return;

        for (int line = 0; line < points.Count - 1; line++)
        {
            _lines.Add(CreateLine(points[line], points[line + 1]));
        }
    }
    private GameObject CreateLine(Vector3 pointA, Vector3 pointB)
    {
        GameObject lineObject = new GameObject($"MeasuringTool:line");
        lineObject.transform.SetParent(transform);

        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        SetLineAppearance(lineRenderer);

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, pointA);
        lineRenderer.SetPosition(1, pointB);

        _linePopups.Add(CreateLinePopup(lineObject, pointA, pointB));

        return lineObject;
    }
    private void SetLineAppearance(LineRenderer lineRenderer)
    {
        lineRenderer.material = _lineMaterial;
        lineRenderer.startWidth = _lineWidth;
        lineRenderer.endWidth = _lineWidth;
    }
    private void UpdateLines()
    {
        foreach (GameObject line in _lines)
        {
            if (line != null)
            {
                Scaler.ScaleLine(_camera, line, _arcWidth);
            }
        }
    }
    private void ReadLineUpdates() { _manager.OnMeasCreated += CreateLines; }
    private void StopLineUpdates() { _manager.OnMeasCreated -= CreateLines; }
    #endregion

    #region Arcs
    [Header("Arcs")]
    [SerializeField] Material _arcMaterial;
    [SerializeField] float _arcWidth = 0.02f;
    private int _arcSegments = 100;
    private float _arcPositionRatio = 0.3f;
    private List<GameObject> _arcs = new List<GameObject>();
    private List<GameObject> _arcPopups = new List<GameObject>();

    public void CreateArcs(List<Vector3> points)
    {
        foreach (GameObject arc in _arcs) Destroy(arc);
        _arcs.Clear();
        ClearArcPopups();

        if (points.Count < 3) return;

        for (int line = 0; line < points.Count - 2; line++)
        {
            CreateArc(points[line], points[line + 1], points[line + 2]);
        }
    }
    private GameObject CreateArc(Vector3 pointA, Vector3 vertex, Vector3 pointB)
    {
        GameObject arcObject = new GameObject($"MeasurementTools:arc");
        _arcs.Add(arcObject);
        arcObject.transform.SetParent(transform);

        LineRenderer arcRenderer = arcObject.AddComponent<LineRenderer>();
        SetArcAppearance(arcRenderer);

        float lengthBA = Vector3.Distance(pointA, vertex);
        float lengthBC = Vector3.Distance(pointB, vertex);
        float shorterLength = Mathf.Min(lengthBA, lengthBC);
        float distanceFromB = shorterLength * _arcPositionRatio;

        Vector3 startPoint = Vector3.MoveTowards(vertex, pointA, distanceFromB);
        Vector3 endPoint = Vector3.MoveTowards(vertex, pointB, distanceFromB);
        Vector3 directionBA = (pointA - vertex).normalized;
        Vector3 directionBC = (pointB - vertex).normalized;
        Vector3 bisector = (directionBA + directionBC).normalized;
        Vector3 controlPoint = vertex + bisector * distanceFromB;

        arcRenderer.positionCount = _arcSegments;
        for (int seg = 0; seg < _arcSegments; seg++)
        {
            float t = seg / (float)(_arcSegments - 1);
            Vector3 bezierPoint = GetQuadraticBezierPoint(t, startPoint, endPoint, controlPoint);
            arcRenderer.SetPosition(seg, bezierPoint);
        }

        _arcPopups.Add(CreateArcPopup(arcObject, pointA, vertex, pointB));

        return arcObject;
    }
    private Vector3 GetQuadraticBezierPoint(float t, Vector3 startPoint, Vector3 endPoint, Vector3 controlPoint)
    {
        return Mathf.Pow(1 - t, 2) * startPoint + 2 * (1 - t) * t * controlPoint + Mathf.Pow(t, 2) * endPoint;
    }
    private void SetArcAppearance(LineRenderer arcRenderer)
    {
        arcRenderer.material = _arcMaterial;
        arcRenderer.startWidth = _arcWidth;
        arcRenderer.endWidth = _arcWidth;
    }
    private void UpdateArcs()
    {
        foreach (GameObject arc in _arcs)
        {
            if (arc != null)
            {
                Scaler.ScaleObject(_camera, arc, _arcWidth);
            }
        }
    }
    private void ReadArcUpdates() { _manager.OnMeasCreated += CreateArcs; }
    private void StopArcUpdates() { _manager.OnMeasCreated -= CreateArcs; }
    #endregion

    #region Popups
    [Header("Measurement Popups")]
    [SerializeField] Material _popupMaterial;
    [SerializeField] float _popupSize = 0.005f;
    [SerializeField] float _fontSize = 20f;

    private GameObject CreatePopup(GameObject obj, Vector3 position, string measText)
    {
        GameObject popup = new GameObject("MeasuringTool:popup");
        popup.transform.SetParent(obj.transform);

        RectTransform rect = popup.AddComponent<RectTransform>();
        rect.transform.localPosition = position;
        rect.localScale = new Vector3(_popupSize, _popupSize, _popupSize);

        Canvas popupCanvas = popup.AddComponent<Canvas>();
        popupCanvas.renderMode = RenderMode.WorldSpace;
        popupCanvas.worldCamera = Camera.main;

        TextMeshProUGUI text = popupCanvas.AddComponent<TextMeshProUGUI>();
        text.fontSize = _fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = UnityEngine.Color.white;
        text.fontSharedMaterial = _popupMaterial;
        text.text = measText;

        return popup;
    }
    private GameObject CreateLinePopup(GameObject line, Vector3 pointA, Vector3 pointB)
    {
        Vector3 popupPosition = (pointA + pointB) / 2;
        float val = MeasCalc.GetLength(pointA, pointB);
        string measText = $"{val:F2}m";
        GameObject popup = CreatePopup(line, popupPosition, measText);
        return popup;
    }
    private void ClearLinePopups()
    {
        foreach (GameObject popup in _linePopups) { Destroy(popup); }
        _linePopups.Clear();
    }
    private GameObject CreateArcPopup(GameObject arc, Vector3 pointA, Vector3 vertex, Vector3 pointB)
    {
        Vector3 popupPosition = vertex;
        float val = MeasCalc.GetAngle(pointA, vertex, pointB);
        string measText = $"{val:F1}°";
        GameObject popup = CreatePopup(arc, popupPosition, measText);
        return popup;
    }
    private void ClearArcPopups()
    {
        foreach (GameObject popup in _arcPopups) { Destroy(popup); }
        _arcPopups.Clear();
    }
    private void UpdatePopups()
    {
        foreach (GameObject popup in _linePopups) 
        {
            if (popup != null)
            {
                popup.transform.forward = _camera.transform.forward;
                Scaler.ScaleText(_camera, popup, _fontSize);
            }
        }
        foreach (GameObject popup in _arcPopups)
        {
            if (popup != null)
            {
                popup.transform.forward = _camera.transform.forward;
                Scaler.ScaleText(_camera, popup, _fontSize);
            }
        }
    }
    #endregion
}