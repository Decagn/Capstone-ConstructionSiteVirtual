using System.Collections.Generic;
using UnityEngine;

public class LessonSnapVisualiser : MonoBehaviour
{
    [SerializeField] private GameObject _snapMarkerPrefab;
    [SerializeField] private float _markerSize = 0.075f;

    [SerializeField] private Material _snapLineMaterial;
    [SerializeField] float _lineWidth = 0.02f;

    private List<GameObject> _markers = new List<GameObject>();
    private List<GameObject> _lines = new List<GameObject>();

    private GameObject CreateMarker(Vector3 point, float size)
    {
        GameObject marker = Instantiate(_snapMarkerPrefab);
        marker.transform.SetParent(transform);
        marker.transform.position = point;
        marker.transform.localScale = new Vector3(size, size, size);
        return marker;
    }
    private void CreateMarkers(List<Vector3> points)
    {
        foreach (Vector3 point in points) _markers.Add(CreateMarker(point, _markerSize));
    }

    private void SetLineAppearance(LineRenderer lineRenderer)
    {
        lineRenderer.material = _snapLineMaterial;
        lineRenderer.startWidth = _lineWidth;
        lineRenderer.endWidth = _lineWidth;
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

        return lineObject;
    }
    private void CreateLines(List<Vector3> points)
    {
        if (points.Count < 2) return;

        for (int line = 0; line < points.Count - 1; line++)
        {
            _lines.Add(CreateLine(points[line], points[line + 1]));
        }
    }

    public void DrawLessonSnapObjects(List<List<Vector3>> objects)
    {
        foreach (GameObject marker in _markers) Destroy(marker);
        _markers.Clear();

        foreach (GameObject line in _lines) Destroy(line);
        _lines.Clear();

        foreach (List<Vector3> objectVertices in objects)
        {
            if (objectVertices.Count == 0) MeasDebug.Log("Object has no vertices, therefore not drawn", "LessonSnapVisualiser");
            CreateMarkers(objectVertices);
            CreateLines(objectVertices);
        }
    }
}