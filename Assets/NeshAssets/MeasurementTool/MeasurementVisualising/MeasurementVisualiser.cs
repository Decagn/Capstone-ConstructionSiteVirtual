using System.Collections.Generic;
using UnityEngine;

/*
 * Measurement Visualiser
 * This class handles the drawing of measurement points, lines, arcs, and pop-up text in the world.
 */
public class MeasurementVisualiser : MonoBehaviour
{
    [SerializeField] MeasurementToolManager _manager;
    [SerializeField] Camera _camera;

    [SerializeField] private NewPointMarkerVisualiser _newPointMarkerVisualiser;
    [SerializeField] private PointMarkerVisualiser _pointMarkerVisualiser;
    [SerializeField] private LineVisualiser _lineVisualiser;
    [SerializeField] private ArcVisualiser _arcVisualiser;
    [SerializeField] private PopupTextVisualiser _popupTextsVisualiser;

    List<IVisualisedObject> _visualisedObjects = new List<IVisualisedObject>();
    GameObject _areaPopupsParent;

    private void Awake()
    {
        _manager.OnSelectPoint += VisualisePointSelection;
        _manager.OnDeSelectPoint += VisualisePointDeselection;
        _manager.OnResetPoints += VisualisePointsReset;
        _manager.OnToolSwitch += VisualisePointsReset;
        _manager.OnMeasurementCreated += VisualiseMeasurement;
        _manager.OnInvalidMeasurementCreated += HandleInvalidMeasurement;
    }
    
    private void OnDisable()
    {
        _manager.OnSelectPoint -= VisualisePointSelection;
        _manager.OnDeSelectPoint -= VisualisePointDeselection;
        _manager.OnResetPoints -= VisualisePointsReset;
        _manager.OnToolSwitch -= VisualisePointsReset;
        _manager.OnMeasurementCreated -= VisualiseMeasurement;
        _manager.OnInvalidMeasurementCreated -= HandleInvalidMeasurement;
    }
   
    private void LateUpdate() { ScaleVisuals(); }

    // New point markers, unlike the other visuals, are created upon a point selection
    // rather than a measurement creation.
    private void CreateNewPointMarkers(List<Vector3> points)
    {
        _visualisedObjects.AddRange(_newPointMarkerVisualiser.Create(points));
    }

    private void CreateVisuals(List<Vector3> points)
    {
        _visualisedObjects.AddRange(_pointMarkerVisualiser.Create(points));
        _visualisedObjects.AddRange(_lineVisualiser.Create(points));
        _visualisedObjects.AddRange(_arcVisualiser.Create(points));

        // Pop-up texts require the other visualised objects to be instantiated.
        _visualisedObjects.AddRange(_popupTextsVisualiser.CreatePopupTexts(_visualisedObjects));
        if (_areaPopupsParent == null)
            _areaPopupsParent = new GameObject("Area Pop-up Texts");
        _visualisedObjects.AddRange(_popupTextsVisualiser.CreateAreaTextPopups(points, _areaPopupsParent));
    }

    private void ClearVisuals()
    {
        foreach (IVisualisedObject obj in _visualisedObjects)
            Destroy(obj.obj);
        _visualisedObjects.Clear();
    }

    // Scales the visualised objects based on how far they are from the player/camera.
    private void ScaleVisuals()
    {
        foreach (IVisualisedObject obj in _visualisedObjects)
            if (obj.visualType == VisualType.NewPointMarker)
                _newPointMarkerVisualiser.Scale(obj, _camera);

            else if (obj.visualType == VisualType.PointMarker)
                _pointMarkerVisualiser.Scale(obj, _camera);

            else if (obj.visualType == VisualType.Line)
                _lineVisualiser.Scale(obj, _camera);

            else if (obj.visualType == VisualType.Arc)
                _arcVisualiser.Scale(obj, _camera);

            else if (obj.visualType == VisualType.PopupText)
            {
                _popupTextsVisualiser.Scale(obj, _camera);
                // ensure text always faces user.
                obj.obj.transform.forward = _camera.transform.forward;
            }

            else
                continue;
    }

    private void VisualisePointSelection(List<Vector3> points)
    {
        CreateNewPointMarkers(points);
    }

    private void VisualisePointDeselection(List<Vector3> points)
    {
        ClearVisuals();
        CreateNewPointMarkers(points);

        if (_manager.GetActiveTool.Name == "Continuous Measurement Tool")
            CreateVisuals(points);
    }

    private void VisualisePointsReset()
    {
        ClearVisuals();
    }

    private void VisualiseMeasurement(List<Vector3> points)
    {
        ClearVisuals();
        CreateVisuals(points);
    }

    private void HandleInvalidMeasurement(List<Vector3> points)
    {
        ClearVisuals();
        CreateNewPointMarkers(points);
    }
}