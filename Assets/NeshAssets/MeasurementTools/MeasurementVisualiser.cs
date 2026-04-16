using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MeasurementVisualiser : MonoBehaviour
{
    private ToolManager _toolManager;

    private Material _lineMaterial;
    private float _lineThickness = 0.02f;
    private Color _lineColor = Color.green;
    private List<GameObject> _lineRendererObjects;

    /*
     * SYSTEM DESIGN
     * arcSegments determines the resolution on the arcs drawn between lines when measuring angles.
     * where more arcSegments = smoother arcs.
     * 
     * arcPositionRatio determines how far up the lines is the arc drawn.
     * arcPoistionRatio always has to be between 0 and 1
     * for example, 0.3 means the arc will be drawn at 30% mark of whichever line is shorter.
     */
    private Material _arcMaterial;
    private List<GameObject> _arcRendererObjects;
    private int _arcSegments = 100;
    private float _arcPositionRatio = 0.3f;

    private GameObject _pointMarkerPrefab;
    private List<GameObject> _pointMarkers;

    public void Initialise(ToolManager toolManager, GameObject pointMarkerPrefab, Material lineMaterial, Material arcMaterial)
    {
        ConnectToolManager(toolManager);
        _toolManager.activeTool.OnSelectedPointsUpdated += VisualiseMeasurements;
        _toolManager.activeTool.OnSelectedPointsReseted += ClearAllVisualisations;

        _pointMarkers = new List<GameObject>();
        SetPointMarkerPrefab(pointMarkerPrefab);

        _lineMaterial = lineMaterial;
        _lineRendererObjects = new List<GameObject>();

        _arcMaterial = arcMaterial;
        _arcRendererObjects = new List<GameObject>();
    }

    private void OnDisable()
    {
        _toolManager.activeTool.OnSelectedPointsUpdated -= VisualiseMeasurements;
        _toolManager.activeTool.OnSelectedPointsReseted -= ClearAllVisualisations;
    }

    private void DrawLine(int lineNum, Vector3 pointA, Vector3 pointB)
    {
        GameObject lineObject = new GameObject($"MeasurementTools:lineRenderer {lineNum}");
        _lineRendererObjects.Add(lineObject);
        lineObject.transform.SetParent(transform);

        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        SetLineAppearance(lineRenderer);

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, pointA);
        lineRenderer.SetPosition(1, pointB);
    }

    private void DrawAllLines(List<Vector3> selectedPoints)
    {
        if (selectedPoints.Count < 2)
        {
            return;
        }

        for (int line = 0; line < selectedPoints.Count - 1; line++)
        {
            DrawLine(line, selectedPoints[line], selectedPoints[line + 1]);
        }
    }

    /*
     * SYSTEM DESIGN
     * the t parameter represents the normalized time factor.
     * in  this case time = arcSegments,
     * so t would be currentSegment / arcSegments;
     */
    private Vector3 GetQuadraticBezierPoint(float t, Vector3 startPoint, Vector3 endPoint, Vector3 controlPoint)
    {
        return Mathf.Pow(1 - t, 2) * startPoint + 2 * (1 - t) * t * controlPoint + Mathf.Pow(t, 2) * endPoint;
    }

    private void DrawArc(int arcNum, Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        GameObject arcObject = new GameObject($"MeasurementTools:arcRenderer {arcNum}");
        _arcRendererObjects.Add(arcObject);
        arcObject.transform.SetParent(transform);

        LineRenderer arcRenderer = arcObject.AddComponent<LineRenderer>();
        SetArcAppearance(arcRenderer);

        float lengthBA = Vector3.Distance(pointA, pointB);
        float lengthBC = Vector3.Distance(pointC, pointB);
        float shorterLength = Mathf.Min(lengthBA, lengthBC);
        float distanceFromB = shorterLength * _arcPositionRatio;

        Vector3 startPoint = Vector3.MoveTowards(pointB, pointA, distanceFromB);
        Vector3 endPoint = Vector3.MoveTowards(pointB, pointC, distanceFromB);
        Vector3 directionBA = (pointA - pointB).normalized;
        Vector3 directionBC = (pointC - pointB).normalized;
        Vector3 bisector = (directionBA + directionBC).normalized;
        Vector3 controlPoint = pointB + bisector * distanceFromB;

        arcRenderer.positionCount = _arcSegments;
        for (int seg = 0; seg < _arcSegments; seg++)
        {
            float t = seg / (float) (_arcSegments - 1);
            Vector3 bezierPoint = GetQuadraticBezierPoint(t, startPoint, endPoint, controlPoint);
            arcRenderer.SetPosition(seg, bezierPoint);
        }
    }

    private void DrawAllArcs(List<Vector3> selectedPoints)
    {
        if (selectedPoints.Count < 3)
        {
            return;
        }

        for (int arc = 0;  arc < selectedPoints.Count - 2; arc++)
        {
            DrawArc(arc, selectedPoints[arc], selectedPoints[arc + 1], selectedPoints[arc + 2]);
        }
    }

    private void DrawPointMarkers(List<Vector3> selectedPoints)
    {
        foreach (GameObject pointMarker in _pointMarkers)
        {
           Destroy(pointMarker);
        }
        _pointMarkers.Clear();
        foreach (Vector3 point in selectedPoints)
        {
            GameObject pointMarker = Instantiate(_pointMarkerPrefab);
            pointMarker.transform.position = point;
            _pointMarkers.Add(pointMarker);
        }
    }

    private void VisualiseMeasurements()
    {
        ClearAllVisualisations();
        List<Vector3> selectedPoints = _toolManager.activeTool.SelectedPoints;
        DrawPointMarkers(selectedPoints);
        DrawAllLines(selectedPoints);
        DrawAllArcs(selectedPoints);
    }

    private void ClearAllVisualisations()
    {
        foreach (GameObject lineObject in _lineRendererObjects)
        {
            Destroy(lineObject);
        }
        _lineRendererObjects.Clear();

        foreach (GameObject arcObject in _arcRendererObjects)
        {
            Destroy(arcObject);
        }
        _arcRendererObjects.Clear();

        foreach (GameObject pointMarker in _pointMarkers)
        {
            Destroy(pointMarker);
        }
        _pointMarkers.Clear();
    }

    #region UTILITY METHODS
    private void ConnectToolManager(ToolManager manager)
    {
        _toolManager = manager;
    }

    private void SetPointMarkerPrefab(GameObject pointMarkerPrefab)
    {
        _pointMarkerPrefab = pointMarkerPrefab;
    }

    private void SetLineAppearance(LineRenderer lineRenderer)
    {
        lineRenderer.material = _lineMaterial;
        lineRenderer.startWidth = _lineThickness;
        lineRenderer.endWidth = _lineThickness;
        lineRenderer.startColor = _lineColor;
        lineRenderer.endColor = _lineColor;
    }

    private void SetArcAppearance(LineRenderer arcRenderer)
    {
        arcRenderer.material = _arcMaterial;
        arcRenderer.startWidth = _lineThickness;
        arcRenderer.endWidth = _lineThickness;
        arcRenderer.startColor = _lineColor;
        arcRenderer.endColor = _lineColor;
    }
    #endregion
}
