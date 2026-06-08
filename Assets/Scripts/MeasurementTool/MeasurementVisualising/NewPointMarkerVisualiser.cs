using System.Collections.Generic;
using UnityEngine;

// New point markers are placed before a measurement is made.
public class NewPointMarkerVisualiser : MonoBehaviour
{
    [SerializeField] private GameObject _newPointMarkerPrefab;
    [SerializeField] private float _newPointMarkerSize = 0.075f;

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
            GameObject pointObj = CreatePointMarker(point, _newPointMarkerPrefab, _newPointMarkerSize);
            IVisualisedObject pointMarker = new IVisualisedObject(VisualType.NewPointMarker, pointObj, new List<Vector3> {point});
            pointMarkers.Add(pointMarker);
        }

        return pointMarkers;
    }

    public void Scale(IVisualisedObject obj, UnityEngine.Camera camera)
    { Scaler.ScaleObject(camera, obj.obj, _newPointMarkerSize); }
}
