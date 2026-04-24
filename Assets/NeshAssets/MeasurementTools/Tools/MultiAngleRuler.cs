using System.Collections.Generic;
using UnityEngine;

public class MultiAngleRuler : MonoBehaviour, IMeasurementTool
{
    public string ToolName
    {
        get => "Multi-angle Ruler";
    }
    public Sprite ToolIcon { get; set; }
    [SerializeField] private Sprite Icon;
    public MeasurementType CurrentMeasurement { get; set; }
    public List<MeasurementType> StoredMeasurements { get; set; }

    public void Awake()
    {
        SetIcon(Icon);
        CurrentMeasurement = new Angle(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        StoredMeasurements = new List<MeasurementType>();
    }

    public bool CreateMeasurement(List<Vector3> points)
    {
        if (points.Count < 2) return false;

        Length newLength = new Length(points[points.Count - 1], points[points.Count - 2]);
        CurrentMeasurement = newLength;
        StoredMeasurements.Add(CurrentMeasurement);

        return true;
    }

    public void SetIcon(Sprite icon)
    {
        ToolIcon = icon;
    }
}
