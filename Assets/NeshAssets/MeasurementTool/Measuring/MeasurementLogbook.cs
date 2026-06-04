using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/*
 * Measurement Logbook
 * This class keeps track of all the measurements taken.
 * Can be used in the future to display past measurements.
 */
public class MeasurementLogbook
{
    private List<Measurement> _logbookOfMeasurements = new List<Measurement>();

    public bool Add(Measurement measurement)
    {
        if (_logbookOfMeasurements == null) 
            return false;

        _logbookOfMeasurements.Add(measurement);
        DisplayMeasurement(measurement);

        return true;
    }

    // Displays information about the measurement
    // and is called automatically when a measurement is added to the logbook.
    private void DisplayMeasurement(Measurement measurement)
    {
        string tool = $"{measurement.GetToolType()}";

        string listPoints = "";
        List<Vector3> points = measurement.GetPoints();

        foreach (Vector3 point in points)
            listPoints += $"\n{point}";

        string listQuantities = "";
        List<IQuantity> quantities = measurement.GetQuantities();

        foreach (IQuantity quant in quantities)
            listQuantities += $"\n{quant.Value} {quant.Unit}";

        DebugMeasurementTools.Log($"Measurement created: \n{tool}{listPoints}{listQuantities}", "MeasurementLogbook");
    }
}
