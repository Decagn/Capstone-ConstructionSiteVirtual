using System.Collections.Generic;
using UnityEngine;

/*
 * [Measuring Tool] Protractor 
 * 
 * Takes angles using three selected points.
 */
public class Protractor : MonoBehaviour, IMeasurementTool
{
    public string Name { get => "Protractor"; }
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
        if (SelectedPoints.Count == 3)
            SelectedPoints.Clear();

        if (SelectedPoints == null)
            SelectedPoints = new List<Vector3>();

        SelectedPoints.Add(point);

        // Need at least three points to create an angle.
        if (SelectedPoints.Count < 3)
            return new InvalidMeasurement();

        return TakeMeasurement();
    }
    public IMeasurement TakeMeasurement()
    {
        Angle angle = new Angle(SelectedPoints[0], SelectedPoints[1], SelectedPoints[2]);

        Measurement measurement = new Measurement(this, SelectedPoints, angle);
        return measurement;
    }

    public void SetIcon() { ToolIcon = _icon; }
}
