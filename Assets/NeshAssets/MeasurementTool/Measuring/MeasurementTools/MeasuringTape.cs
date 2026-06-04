using System.Collections.Generic;
using UnityEngine;

/*
 * [Measuring Tool] Measuring Tape
 * 
 * Takes lengths between two selected points.
 */
public class MeasuringTape : MonoBehaviour, IMeasurementTool
{
    public string Name { get => "Measuring Tape";  }
    public Sprite ToolIcon { get; set; }
    public List<Vector3> SelectedPoints { get; set; }

    // Attach the sprite for this tool's icon in the editor.
    [SerializeField] private Sprite _icon;

    private void Awake()
    {
        SetIcon();
        SelectedPoints = new List<Vector3>();
    }

    public IMeasurement TakePoint(Vector3 point)
    {
        if (SelectedPoints == null)
            SelectedPoints = new List<Vector3>();

        SelectedPoints.Add(point);

        // Need at least two points to create a length.
        if (SelectedPoints.Count < 2) 
            return new InvalidMeasurement();

        IMeasurement measurement = TakeMeasurement();

        SelectedPoints.Clear();
        return measurement;
    }
    public IMeasurement TakeMeasurement()
    {
        Length length = new Length(SelectedPoints[0], SelectedPoints[1]);

        Measurement measurement = new Measurement(this, SelectedPoints, length);
        return measurement;
    }

    public void SetIcon() { ToolIcon = _icon; }
}
