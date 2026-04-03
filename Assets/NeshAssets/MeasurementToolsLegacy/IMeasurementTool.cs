using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * SYSTEM DESIGN
 * The MeasurementCollection class caches the measurements computed thus far.
 * 
 * LengthsBetweenPoints[n] = euclidean distance between SelectedPoints[n] and SelectedPoint[n + 1].
 *  
 * AnglesBetweenLengths[n] = angle between line A and line B, where:
 * line A is the line from SelectedPoints[n] to SelectedPoints[n + 1] and
 * line B is the line from SelectedPoints[n + 1] to SelectedPoints[n + 2]
 * 
 * Therefore, size of the vector SelectedPoints == LengthBetweenPoints + 1 == AnglesBetweenLengths + 2
 */
public class MeasurementCollection
{
    public List<Vector3> SelectedPoints;
    public List<float> LengthsBetweenPoints;
    public List<float> AnglesBetweenLengths;
}

public interface IMeasurementTool
{
    string ToolName {  get; }
    void OnPointSelected(Vector3 point);
    event Action<MeasurementCollection> OnMeasurementsUpdated;
    void ResetMeasurements();
}
