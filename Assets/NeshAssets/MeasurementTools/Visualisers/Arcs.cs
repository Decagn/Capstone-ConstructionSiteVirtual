using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Arcs : MonoBehaviour
{
    [SerializeField] private Material _arcMaterial;
    [SerializeField] private float _arcThickness = 0.02f;
    [SerializeField] private Color _arcColor = Color.green;

    [SerializeField] private float _popupSize = 0.005f;
    [SerializeField] private Material _popupMaterial;

    /*
     * SYSTEM DESIGN
     * arcSegments determines the resolution on the arcs drawn between lines when measuring angles.
     * where more arcSegments = smoother arcs.
     * 
     * arcPositionRatio determines how far up the lines is the arc drawn.
     * arcPoistionRatio always has to be between 0 and 1
     * for example, 0.3 means the arc will be drawn at 30% mark of whichever line is shorter.
     */
    private int _arcSegments = 100;
    private float _arcPositionRatio = 0.3f;

    private List<GameObject> _arcs = new List<GameObject>();
    private List<GameObject> _popups = new List<GameObject>();

    private void LateUpdate()
    {
        foreach (GameObject popup in _popups)
        {
            popup.transform.forward = Camera.main.transform.forward;
        }
    }

    public void CreateArcs(List<Vector3> points)
    {
        Clear();
        if (points.Count < 3) return;

        for (int line = 0; line < points.Count - 2; line++)
        {
            CreateAt(points[line], points[line + 1], points[line + 2]);
        }
    }
    private void CreateAt(Vector3 pointA, Vector3 vertex, Vector3 pointB)
    {
        GameObject arcObject = new GameObject($"MeasurementTools:arc");
        _arcs.Add(arcObject);
        arcObject.transform.SetParent(transform);

        LineRenderer arcRenderer = arcObject.AddComponent<LineRenderer>();
        SetLineAppearance(arcRenderer);

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
            float t = seg / (float) (_arcSegments - 1);
            Vector3 bezierPoint = GetQuadraticBezierPoint(t, startPoint, endPoint, controlPoint);
            arcRenderer.SetPosition(seg, bezierPoint);
        }

        CreatePopup(arcObject, pointA, vertex, pointB);
    }
    private void CreatePopup(GameObject line, Vector3 pointA, Vector3 vertex, Vector3 pointB)
    {
        GameObject popup = new GameObject("MeasurementTool:Visualiser:LinePopup");
        popup.transform.SetParent(line.transform);
        _popups.Add(popup);

        RectTransform rect = popup.AddComponent<RectTransform>();
        rect.transform.localPosition = vertex;
        rect.localScale = new Vector3(_popupSize, _popupSize, _popupSize);

        Canvas popupCanvas = popup.AddComponent<Canvas>();
        popupCanvas.sortingOrder = 100;
        popupCanvas.renderMode = RenderMode.WorldSpace;
        popupCanvas.worldCamera = Camera.main;

        TextMeshProUGUI text = popupCanvas.AddComponent<TextMeshProUGUI>();
        text.fontSize = 20;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        float measurement = Calculator.MeasureAngle(pointA, vertex, pointB);
        text.text = $"{measurement:F1}°";
        text.fontSharedMaterial = _popupMaterial;
    }
    public void Clear()
    {
        foreach (GameObject arc in _arcs)
        {
            Destroy(arc);
        }
        _arcs.Clear();
        foreach (GameObject popup in _popups)
        {
            Destroy(popup);
        }
        _popups.Clear();
    }

    private void SetLineAppearance(LineRenderer lineRenderer)
    {
        lineRenderer.material = _arcMaterial;
        lineRenderer.startWidth = _arcThickness;
        lineRenderer.endWidth = _arcThickness;
        lineRenderer.startColor = _arcColor;
        lineRenderer.endColor = _arcColor;
    }
    private Vector3 GetQuadraticBezierPoint(float t, Vector3 startPoint, Vector3 endPoint, Vector3 controlPoint)
    {
        return Mathf.Pow(1 - t, 2) * startPoint + 2 * (1 - t) * t * controlPoint + Mathf.Pow(t, 2) * endPoint;
    }
}
