using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MeasLogBook
{
    private List<Measurement> _measurements = new List<Measurement>();

    public bool Add(Measurement measurement)
    {
        if (_measurements == null) return false;

        _measurements.Add(measurement);
        announceMeas(measurement);
        return true;
    }

    private void announceMeas(Measurement meas)
    {
        string tool = $"{meas.GetToolType()}";

        string listPoints = "";
        List<Vector3> points = meas.GetPoints();
        foreach (Vector3 point in points)
        {
            listPoints += $"\n{point}";
        }

        string listQuantities = "";
        List<IQuantity> quantities = meas.GetQuantities();
        foreach (IQuantity quant in quantities)
        {
            listQuantities += $"\n{quant.Value} {quant.Unit}";
        }

        MeasDebug.Log($"Measurement created: \n{tool}{listPoints}{listQuantities}", "MeasLogBook");
    }
}
