using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ProtractorRuler : MonoBehaviour, IMeasurementTool
{
    private List<Vector3> _selectedPoints;
    public string ToolName => "Protractor Ruler";
    public event Action<MeasurementCollection> OnMeasurementsUpdated;

    public void OnPointSelected(Vector3 point)
    {
        _selectedPoints.Add(point);
        if (_selectedPoints.Count >= 2)
        {
            BroadcastMeasurements();
        }
    }

    public void ResetMeasurements()
    {
        _selectedPoints.Clear();
    }

    private void BroadcastMeasurements()
    {
        List<List<float>> computedMeasurements = MeasurementComputer.ComputeDistancesAndAngles(_selectedPoints);
        MeasurementCollection measurementCollection = new MeasurementCollection
        {
            SelectedPoints = _selectedPoints,
            LengthsBetweenPoints = computedMeasurements[0],
            AnglesBetweenLengths = computedMeasurements[1]
        };
        OnMeasurementsUpdated?.Invoke(measurementCollection);
    }
}
