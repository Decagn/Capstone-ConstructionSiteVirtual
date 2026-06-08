using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
 *  IMeasuremetTool interface
 *  All measurement tools must implement this interface.
 */
public interface IMeasurementTool
{
    string Name { get; }
    Sprite ToolIcon { get; set; }
    
    // Points selected while this tool is active.
    List<Vector3> SelectedPoints { get; set; }

    /* 
    * Dictates what to do with the selected point
    * and should call TakeMeasurement() automatically
    * when the number of selected points are adequate to produce the intended quantity.
    */
    IMeasurement TakePoint(Vector3 point);
    // Dictates what kind of quantity (length, angle, etc.) the tool produces from the selected points.
    IMeasurement TakeMeasurement();

    bool RemoveLastSelectedPoint()
    {
        if (SelectedPoints == null || SelectedPoints.Count == 0) return false;

        SelectedPoints.RemoveAt(SelectedPoints.Count - 1);
        return true;
    }
    bool ResetAllSelectedPoints()
    {
        if (SelectedPoints == null || SelectedPoints.Count == 0) return false;

        SelectedPoints.Clear();
        return true;
    }
    
    // Sets a Sprite to the ToolIcon field at runtime.
    void SetIcon();
}
