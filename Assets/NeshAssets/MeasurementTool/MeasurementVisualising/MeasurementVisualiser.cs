using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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

    private void Awake()
    {
        _manager.OnSelectPoint += CreateNewPointMarkers;
        _manager.OnMeasurementCreated += CreateVisuals;
        _manager.OnMeasurementCleared += ClearVisuals;
    }
    
    private void OnDisable()
    {
        _manager.OnSelectPoint -= CreateNewPointMarkers;
        _manager.OnMeasurementCreated -= CreateVisuals;
        _manager.OnMeasurementCleared -= ClearVisuals;
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
        ClearVisuals();

        _visualisedObjects.AddRange(_pointMarkerVisualiser.Create(points));
        _visualisedObjects.AddRange(_lineVisualiser.Create(points));
        _visualisedObjects.AddRange(_arcVisualiser.Create(points));

        // Pop-up texts require the other visualised objects to be instantiated.
        _visualisedObjects.AddRange(_popupTextsVisualiser.CreatePopupTexts(_visualisedObjects));

        // Area pop-up texts get the areas directly from the tool since recalculating them is costly.
        IMeasurementTool activeTool = _manager.GetActiveTool;
        // Area pop-ups only should be created for the continuous measurement tool.
        if (activeTool.Name != "Continuous Measurement Tool")
            return;
        // Area pop-ups only should be created if the continous measurement has been initialised.
        if (!((ContinuousMeasurementTool)activeTool).IsContinuousMeasurementInitialised())
            return;

        Measurement currConitnuousMeasurement = ((ContinuousMeasurementTool)activeTool).GetContinuousMeasurement();
        GameObject areaPopupsParent = new GameObject("Area Pop-up Texts");
        _visualisedObjects.AddRange(_popupTextsVisualiser.CreateAreaTextPopups(currConitnuousMeasurement, areaPopupsParent));
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
}