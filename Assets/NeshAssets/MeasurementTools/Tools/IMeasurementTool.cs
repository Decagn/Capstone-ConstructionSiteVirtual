using NUnit.Framework;
using System;
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
    bool CreateMeasurement(List<Vector3> points);
    void SetIcon(Sprite icon);
}
