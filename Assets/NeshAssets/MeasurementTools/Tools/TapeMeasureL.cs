using System.Collections.Generic;
using UnityEngine;

public class TapeMeasureL : MonoBehaviour, IMeasurementTool
{
    public string ToolName
    {
        get => "Tape Measure";
    }
    public Sprite ToolIcon { get; set; }
    [SerializeField] private Sprite Icon;
    public MeasurementType CurrentMeasurement { get; set; }
    public List<MeasurementType> StoredMeasurements {get; set;}

    public void Awake()
    {
        SetIcon(Icon);
        CurrentMeasurement = new Lengths(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        StoredMeasurements = new List<MeasurementType>();
    }

    public bool CreateMeasurement(List<Vector3> points)
    {
        if (points.Count < 2) return false;

        Lengths newLength = new Lengths(points[0], points[1]);
        CurrentMeasurement = newLength;
        StoredMeasurements.Add(newLength);
        return true;
    }

    public void SetIcon(Sprite icon)
    {
        ToolIcon = icon;
    }
}
