using System.Collections.Generic;
using UnityEngine;

/*
 * [Measuring Tool] Continuous Measurement Tool 
 * 
 * Takes lengths every two points selected
 * and takes angles for every three points,
 * and takes areas of closed polygons.
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
    private List<Area> TakeAreas()
    {
        List<List<Vector3>> polygons = new List<List<Vector3>>();
        for (int p = 0; p < SelectedPoints.Count - 3; p++)
        {
            List<Vector3> polygon = new List<Vector3>();
            // finds the next index where the same point was selected.
            // the next index has to be at minimum 3 away so it can at least form a triangle.
            for (int p2 = SelectedPoints.Count - 1; p2 > p + 2; p2--)
            {
                if (SelectedPoints[p2] == SelectedPoints[p])
                {
                    polygon.AddRange(SelectedPoints.GetRange(p, p2 - p));
                    polygons.Add(polygon);
                }
            }
        }

        List<Area> areas = new List<Area>();
         foreach (List<Vector3> polygon  in polygons)
            areas.Add(new Area(polygon));
         return areas;
    }
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

        if (SelectedPoints.Count >= 2)
            _continuousMeasurement.AddQuantity(TakeLength());
        if (SelectedPoints.Count >= 3)
            _continuousMeasurement.AddQuantity(TakeAngle());
        if (SelectedPoints.Count < 4)
            return _continuousMeasurement;

        List<Area> areas = TakeAreas();
        foreach (Area area in areas)
        {
            _continuousMeasurement.AddQuantity(area);
            //DebugMeasurementTools.Log($"Area:{area.Value}{area.Unit}", "ContinouousMeasurementTool");
        }

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

    public bool RemoveLastSelectedPoint()
    {
        if (SelectedPoints == null || SelectedPoints.Count == 0) 
            return false;

        SelectedPoints.RemoveAt(SelectedPoints.Count - 1);

        return true;
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
    public Measurement GetContinuousMeasurement() => _continuousMeasurement;
    public bool IsContinuousMeasurementInitialised() => _continuousMeasurementInitialised;
}
