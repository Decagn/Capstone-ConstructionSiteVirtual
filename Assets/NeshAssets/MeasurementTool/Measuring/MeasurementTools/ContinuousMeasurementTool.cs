using System.Collections.Generic;
using UnityEngine;

/*
 * [Measuring Tool] Continuous Measurement Tool 
 * 
 * Takes lengths every two points selected
 * and takes angles for every three points.
 * Allows the user to keep selecting new points
 * which connect to the previous points.
 */
public class ContinuousMeasurementTool : MonoBehaviour, IMeasurementTool
{
    public string Name { get => "Continuous Measurement Tool"; }
    public Sprite ToolIcon { get; set; }
    public List<Vector3> SelectedPoints { get; set; }

    // Attach the sprite for this tool's icon in the editor.
    [SerializeField] Sprite icon;

    // One measurement stores all the sub-measurements. 
    private Measurement _continuousMeasurement;
    private bool _continuousMeasurementInitialised = false;

    private void Awake()
    {
        SetIcon();
        SelectedPoints = new List<Vector3>();
    }

    private Length TakeLength() => new Length(
        SelectedPoints[SelectedPoints.Count - 1],
        SelectedPoints[SelectedPoints.Count - 2]);
    private Angle TakeAngle() => new Angle(
        SelectedPoints[SelectedPoints.Count - 1],
        SelectedPoints[SelectedPoints.Count - 2],
        SelectedPoints[SelectedPoints.Count - 3]);
    public IMeasurement TakeMeasurement()
    {
        if (!_continuousMeasurementInitialised)
        {
            Length length = TakeLength();
            _continuousMeasurement = new Measurement(this, SelectedPoints, length);
            _continuousMeasurementInitialised = true;
            return _continuousMeasurement;
        }

        _continuousMeasurement.AddPoint(SelectedPoints[SelectedPoints.Count - 1]);
        _continuousMeasurement.AddQuantity(TakeLength());
        _continuousMeasurement.AddQuantity(TakeAngle());

        return _continuousMeasurement;
    }
    public IMeasurement TakePoint(Vector3 point)
    {
        if (SelectedPoints == null)
            SelectedPoints = new List<Vector3>();

        SelectedPoints.Add(point);

        // Need at least two points to create a length.
        if (SelectedPoints.Count < 2) 
            return new InvalidMeasurement();

        return TakeMeasurement();
    }

    public bool ResetAllSelectedPoints()
    {
        if (SelectedPoints == null || SelectedPoints.Count == 0) 
            return false;

        SelectedPoints.Clear();

        _continuousMeasurementInitialised = false;

        return true;
    }

    public void SetIcon() { ToolIcon = icon; }
}
