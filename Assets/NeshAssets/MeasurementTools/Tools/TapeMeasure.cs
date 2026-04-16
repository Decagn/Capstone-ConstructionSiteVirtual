using System.Collections.Generic;
using UnityEngine;

public class TapeMeasure : MonoBehaviour, IMeasurementTool
{
    public string ToolName
    {
        get => "Tape Measure";
    }
    public Sprite ToolIcon { get; set; }
    public MeasurementType CurrentMeasurement { get; set; }
    public List<MeasurementType> StoredMeasurements {get; set;}

    public void Initialise(Sprite toolIcon)
    {
        ToolIcon = toolIcon;
        CurrentMeasurement = new Length(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        StoredMeasurements = new List<MeasurementType>();
    }

    public bool CreateMeasurement(List<Vector3> points)
    {
        if (points.Count < 2) return false;

        Length newLength = new Length(points[0], points[1]);
        CurrentMeasurement = newLength;
        StoredMeasurements.Add(newLength);
        return true;
    }
}
