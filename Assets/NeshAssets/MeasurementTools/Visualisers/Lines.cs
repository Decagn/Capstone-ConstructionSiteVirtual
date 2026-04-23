using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Lines : MonoBehaviour
{
    [SerializeField] private Material _lineMaterial;
    [SerializeField] private float _lineThickness = 0.02f;
    [SerializeField] private Color _lineColor = Color.green;

    [SerializeField] private float _popupSize = 0.01f;

    private List<GameObject> _lines = new List<GameObject>();
    private List<GameObject> _popups = new List<GameObject>();

    private void LateUpdate()
    {
        foreach(GameObject popup in _popups)
        {
            popup.transform.forward = Camera.main.transform.forward;
        }
    }

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
        
        CreatePopup(lineObject, pointA, pointB);
    }
    private void CreatePopup(GameObject line, Vector3 pointA, Vector3 pointB)
    {
        GameObject _popup = new GameObject("MeasurementTool:Visualiser:LinePopup");
        _popup.transform.SetParent(line.transform);
        _popups.Add(_popup);

        RectTransform rect = _popup.AddComponent<RectTransform>();
        rect.transform.localPosition = (pointA + pointB) / 2;
        rect.localScale = new Vector3(_popupSize, _popupSize, _popupSize);

        Canvas _popupCanvas = _popup.AddComponent<Canvas>();
        _popupCanvas.sortingOrder = 100;
        _popupCanvas.renderMode = RenderMode.WorldSpace;
        _popupCanvas.worldCamera = Camera.main;

        TextMeshProUGUI _text = _popupCanvas.AddComponent<TextMeshProUGUI>();
        _text.fontSize = 20;
        _text.alignment = TextAlignmentOptions.Center;
        _text.color = Color.white;
        float measurement = Calculator.MeasureLength(pointA, pointB);
        _text.text = $"{measurement:F2}m";
    }
    public void Clear()
    {
        foreach (GameObject line in _lines)
        {
            Destroy(line);
        }
        _lines.Clear();
        foreach (GameObject popup in _popups)
        {
            Destroy(popup);
        }
        _popups.Clear();
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
