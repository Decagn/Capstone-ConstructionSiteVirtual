using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * [Measuring Tool] Point Identifier
 * 
 * This tool was used during development to select and identify points in the scene.
 * In the future, can be extended to automatically adding lesson measurement points.
 */
public class PointIndentifier : MonoBehaviour, IMeasurementTool
{
    public string Name { get => "Point Identifier"; }
    public Sprite ToolIcon { get; set; }
    public List<Vector3> SelectedPoints { get; set; }

    // Attach the sprite for this tool's icon in the editor.
    [SerializeField] Sprite icon;

    private void Awake()
    {
        SetIcon();
        SelectedPoints = new List<Vector3>();
    }

    // Never called for the point identifier since no measurement are taken.
    public IMeasurement TakeMeasurement() => new InvalidMeasurement();

    public IMeasurement TakePoint(Vector3 point)
    {
        if (SelectedPoints == null)
            SelectedPoints = new List<Vector3>();

        SelectedPoints.Add(point);
        MeasDebug.Log($"({point.x}, {point.y}, {point.z})", "PointIndentifier");

        return new InvalidMeasurement();
    }

    public void SetIcon() { ToolIcon = icon; }
}