using GLTFast.Schema;
using System.Collections.Generic;
using UnityEngine;

public class LineVisualiser : MonoBehaviour, IVisualiser
{
    [SerializeField] UnityEngine.Material _lineMaterial;
    [SerializeField] float _lineWidth = 0.02f;

    private void SetLineAppearance(LineRenderer lineRenderer)
    {
        lineRenderer.material = _lineMaterial;
        lineRenderer.startWidth = _lineWidth;
        lineRenderer.endWidth = _lineWidth;
    }

    private GameObject CreateLine(Vector3 pointA, Vector3 pointB)
    {
        GameObject lineObject = new GameObject($"Measurement Line");
        lineObject.transform.SetParent(transform);

        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        SetLineAppearance(lineRenderer);

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, pointA);
        lineRenderer.SetPosition(1, pointB);

        return lineObject;
    }

    public List<IVisualisedObject> Create(List<Vector3> points)
    {
        List<IVisualisedObject> lines = new List<IVisualisedObject>();

        if (points.Count < 2) 
            return lines;

        for (int l= 0; l < points.Count - 1; l++)
        {
            GameObject lineObj = CreateLine(points[l], points[l + 1]);
            IVisualisedObject line = new IVisualisedObject(VisualType.Line, lineObj, new List<Vector3> { points[l], points[l + 1] });
            lines.Add(line);
        }

        return lines;
    }

    // Makes lines with a different material.
    // Used by the lesson measurements manager to create custom lines.
    public List<IVisualisedObject> CreateCustom(List<Vector3> points, UnityEngine.Material lineMaterial)
    {
        List<IVisualisedObject> lines = new List<IVisualisedObject>();

        if (points.Count < 2)
            return lines;

        for (int l = 0; l < points.Count - 1; l++)
        {
            GameObject lineObj = CreateLine(points[l], points[l + 1]);
            lineObj.GetComponent<LineRenderer>().material = lineMaterial;
            IVisualisedObject line = new IVisualisedObject(VisualType.Line, lineObj, new List<Vector3> { points[l], points[l + 1] });
            lines.Add(line);
        }

        return lines;
    }

    public void Scale(IVisualisedObject obj, UnityEngine.Camera camera)
    { Scaler.ScaleLine(camera, obj.obj, _lineWidth); }
}
