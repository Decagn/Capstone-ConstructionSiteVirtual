using System.Collections.Generic;
using UnityEngine;

public class LessonMeasurementsPointsVisualiser : MonoBehaviour
{
    [SerializeField] private UnityEngine.Camera _camera;
    [SerializeField] private PointMarkerVisualiser _markerVisualiser;
    [SerializeField] private LineVisualiser _lineVisualiser;

    [SerializeField] private GameObject _lessonPointMarkerPrefab;
    [SerializeField] private UnityEngine.Material _lessonMeasurementLineMaterial;

    private List<IVisualisedObject> _visualisedObjects = new List<IVisualisedObject>();

    private void LateUpdate() { ScaleVisuals(); }

    // Create custom points and lines for the lesson measurements
    // so that they are distinct from the other visualised objects in the world.
    public void DisplayLessonMeasurements(List<List<Vector3>> lessonMeasurements)
    {
        foreach(List<Vector3> measurement in lessonMeasurements)
        {
            _visualisedObjects.AddRange(_markerVisualiser.CreateCustom(measurement, _lessonPointMarkerPrefab));
            _visualisedObjects.AddRange(_lineVisualiser.CreateCustom(measurement, _lessonMeasurementLineMaterial));
        }
    }

    public void ClearMeasurements()
    {
        foreach (IVisualisedObject obj in _visualisedObjects)
            Destroy(obj.obj);
        _visualisedObjects.Clear();
    }

    // Scales the visualised objects based on how far they are from the player/camera.
    private void ScaleVisuals()
    {
        foreach (IVisualisedObject obj in _visualisedObjects)
        {
            if (obj.visualType == VisualType.PointMarker)
                _markerVisualiser.Scale(obj, _camera);

            else if (obj.visualType == VisualType.Line)
                _lineVisualiser.Scale(obj, _camera);

            else
                continue;
        }
    } 
}