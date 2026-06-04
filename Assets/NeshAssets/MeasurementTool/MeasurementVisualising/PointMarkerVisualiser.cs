using GLTFast.Schema;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

// Point Markers are placed where the points of a complete measurement are.
public class PointMarkerVisualiser : MonoBehaviour, IVisualiser
{
    [SerializeField] private GameObject _pointMarkerPrefab;
    [SerializeField] private float _pointMarkerSize = 0.075f;

    private GameObject CreatePointMarker(Vector3 point, GameObject markerPrefab, float size)
    {
        GameObject pointMarker = Instantiate(markerPrefab);

        pointMarker.transform.SetParent(transform);
        pointMarker.transform.position = point;
        pointMarker.transform.localScale = new Vector3(size, size, size);

        return pointMarker;
    }

    public List<IVisualisedObject> Create(List<Vector3> points)
    {
        List<IVisualisedObject> pointMarkers = new List<IVisualisedObject>();

        foreach (Vector3 point in points)
        {
            GameObject pointObj = CreatePointMarker(point, _pointMarkerPrefab, _pointMarkerSize);
            IVisualisedObject pointMarker = new IVisualisedObject(VisualType.PointMarker, pointObj, new List<Vector3> { point });
            pointMarkers.Add(pointMarker);
        }

        return pointMarkers;
    }

    // Makes points with a different prefab.
    // Used by the lesson measurements manager to create custom points.
    public List<IVisualisedObject> CreateCustom(List<Vector3> points, GameObject pointPrefab)
    {
        List<IVisualisedObject> pointMarkers = new List<IVisualisedObject>();

        foreach (Vector3 point in points)
        {
            GameObject pointObj = CreatePointMarker(point, pointPrefab, _pointMarkerSize);
            IVisualisedObject pointMarker = new IVisualisedObject(VisualType.PointMarker, pointObj, new List<Vector3> { point });
            pointMarkers.Add(pointMarker);
        }

        return pointMarkers;
    }

    public void Scale(IVisualisedObject obj, UnityEngine.Camera camera) 
    { Scaler.ScaleObject(camera, obj.obj, _pointMarkerSize); }
}
