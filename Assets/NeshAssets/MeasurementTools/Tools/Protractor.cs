using System.Collections.Generic;
using UnityEngine;

public class Protractor : MonoBehaviour, IMeasurementTool
{
    public string ToolName
    {
        get => "Protractor";
    }
    public Sprite ToolIcon { get; set; }
    public MeasurementType CurrentMeasurement { get; set; }
    public List<MeasurementType> StoredMeasurements { get; set; }

    public void Initialise(Sprite toolIcon)
    {
        ToolIcon = toolIcon;
        CurrentMeasurement = new Angle(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        StoredMeasurements = new List<MeasurementType>();
    }

    public bool CreateMeasurement(List<Vector3> points)
    {
        if (points.Count < 3) return false;

        Angle newAngle = new Angle(points[0], points[1], points[2]);
        CurrentMeasurement = newAngle;
        StoredMeasurements.Add(newAngle);
        return true;
    }
}