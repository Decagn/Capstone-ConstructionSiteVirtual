using System.Collections.Generic;
using UnityEngine;

public class Lines : MonoBehaviour
{
    [SerializeField] private Material _lineMaterial;
    [SerializeField] private float _lineThickness = 0.02f;
    [SerializeField] private Color _lineColor = Color.green;

    private List<GameObject> _lines = new List<GameObject>();

    public void CreateLines(List<Vector3> points)
    {
        Clear();
        if (points.Count < 2) return;

        for (int line = 0; line < points.Count - 1; line++)
        {
            CreateAt(points[line], points[line + 1]);
        }
    }
    private void CreateAt(Vector3 pointA, Vector3 pointB)
    {
        GameObject lineObject = new GameObject($"MeasurementTools:line");
        _lines.Add(lineObject);
        lineObject.transform.SetParent(transform);

        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        SetLineAppearance(lineRenderer);

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, pointA);
        lineRenderer.SetPosition(1, pointB);
    }
    public void Clear()
    {
        foreach (GameObject line in _lines)
        {
            Destroy(line);
        }
        _lines.Clear();
    }

    private void SetLineAppearance(LineRenderer lineRenderer)
    {
        lineRenderer.material = _lineMaterial;
        lineRenderer.startWidth = _lineThickness;
        lineRenderer.endWidth = _lineThickness;
        lineRenderer.startColor = _lineColor;
        lineRenderer.endColor = _lineColor;
    }
}
