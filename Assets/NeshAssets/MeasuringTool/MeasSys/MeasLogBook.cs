using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MeasLogBook : MonoBehaviour
{
    private List<Meas> _measurements = new List<Meas>();

    public bool Add(Meas measurement)
    {
        if (_measurements == null) return false;

        _measurements.Add(measurement);
        return true;
    }
}
