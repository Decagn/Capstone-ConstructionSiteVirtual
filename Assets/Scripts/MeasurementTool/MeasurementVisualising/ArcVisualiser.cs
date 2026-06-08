using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public class ArcVisualiser : MonoBehaviour, IVisualiser
{
    [SerializeField] Material _arcMaterial;
    [SerializeField] float _arcWidth = 0.02f;

    // the more segments, the smoother the arc appears.
    private int _arcSegments = 100;
    // how far up the shorter line is the arc drawn from
    // (i.e. 0.3 = 30% the length of the shorter line from the vertex)
    private float _arcPositionRatio = 0.3f;

    private Vector3 GetQuadraticBezierPoint(float t, Vector3 startPoint, Vector3 endPoint, Vector3 controlPoint) =>
        Mathf.Pow(1 - t, 2) * startPoint + 2 * (1 - t) * t * controlPoint + Mathf.Pow(t, 2) * endPoint;
    
    private void SetArcAppearance(LineRenderer arcRenderer)
    {
        arcRenderer.material = _arcMaterial;
        arcRenderer.startWidth = _arcWidth;
        arcRenderer.endWidth = _arcWidth;
    }

    private GameObject CreateArc(Vector3 pointA, Vector3 vertex, Vector3 pointB)
    {
        GameObject arcObject = new GameObject($"Measurement Arc");
        arcObject.transform.SetParent(transform);

        LineRenderer arcRenderer = arcObject.AddComponent<LineRenderer>();
        SetArcAppearance(arcRenderer);

        // Calculate how far along each line the arc should start/end,
        // using the shorter line so the arc fits within both line segments.
        float lengthBA = Vector3.Distance(pointA, vertex);
        float lengthBC = Vector3.Distance(pointB, vertex);
        float shorterLength = Mathf.Min(lengthBA, lengthBC);
        float distanceFromB = shorterLength * _arcPositionRatio;

        // Place the arc's start and end points at equal distances from the vertex.
        // along each respective line
        Vector3 startPoint = Vector3.MoveTowards(vertex, pointA, distanceFromB);
        Vector3 endPoint = Vector3.MoveTowards(vertex, pointB, distanceFromB);

        // Compute the angle bisector direction to use as the Bezier control point,
        // pulling the arc outward from the vertex to give it a natural curve.
        Vector3 directionBA = (pointA - vertex).normalized;
        Vector3 directionBC = (pointB - vertex).normalized;
        Vector3 bisector = (directionBA + directionBC).normalized;
        Vector3 controlPoint = vertex + bisector * distanceFromB;

        // Sample the quadratic Bezier curve at evenly spaced intervals and
        // assign each point to the LineRenderer to draw the arc.
        arcRenderer.positionCount = _arcSegments;
        for (int seg = 0; seg < _arcSegments; seg++)
        {
            float t = seg / (float)(_arcSegments - 1);
            Vector3 bezierPoint = GetQuadraticBezierPoint(t, startPoint, endPoint, controlPoint);
            arcRenderer.SetPosition(seg, bezierPoint);
        }

        return arcObject;
    }

    public List<IVisualisedObject> Create(List<Vector3> points)
    {
        List<IVisualisedObject> arcs = new List<IVisualisedObject>();

        if (points.Count < 3) 
            return arcs;

        for (int a = 0; a < points.Count - 2; a++)
        {
            GameObject arcObj = CreateArc(points[a], points[a + 1], points[a + 2]);
            IVisualisedObject arc = new IVisualisedObject(VisualType.Arc, arcObj, new List<Vector3> { points[a], points[a + 1], points[a + 2] });
            arcs.Add(arc);
        }

        return arcs;
    }

    public void Scale(IVisualisedObject obj, Camera camera)
    { Scaler.ScaleLine(camera, obj.obj, _arcWidth); }
}
