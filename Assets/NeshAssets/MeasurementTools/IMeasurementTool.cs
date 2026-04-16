using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public interface IMeasurementTool
{
    string ToolName { get; }
    Sprite ToolIcon { get; set; }
    MeasurementType CurrentMeasurement { get; set; }
    List<MeasurementType> StoredMeasurements { get; set; }
    /*
     * [SYSTEM DESIGN]
     * CreateMeasurement returns true if measurement is sucessfully created.
     */
    void Initialise(Sprite toolIcon);
    bool CreateMeasurement(List<Vector3> points);
}
