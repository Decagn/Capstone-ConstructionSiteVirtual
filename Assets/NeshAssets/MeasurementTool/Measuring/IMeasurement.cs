using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using System;

/*
 *  IMeasurement interface
 *  
 *  This is interface was created to allow some functions
 *  to return InvalidMeasurement types
 *  when unable to create valid measurements.
 */
public interface IMeasurement {}

public struct InvalidMeasurement : IMeasurement {}

public struct Measurement : IMeasurement
{
    // The tool that created this measurement.
    private IMeasurementTool _tool;

    // The points that make up this measurement.
    private List<Vector3> _points;

    /*
    * The quantities contained in this measurement.
    * This is a list instead of one quantity since some measurement tools
    * create more than one quantity per measurement.
    */
    private List<IQuantity> _quantities;

    public Measurement(IMeasurementTool tool, List<Vector3> points, IQuantity quantity)
    {
        _tool = tool;

        _points = new List<Vector3>();
        _points.AddRange(points);

        _quantities = new List<IQuantity>();
        _quantities.Add(quantity);
    }

    public bool AddQuantity(IQuantity quantity)
    {
        if (_quantities == null) 
            return false;

        _quantities.Add(quantity);
        return true;
    }
    public bool RemoveLastQuantity()
    {
        if (_quantities == null || _quantities.Count < 1) 
            return false;

        _quantities.RemoveAt(_quantities.Count - 1);
        return true;
    }
    public bool AddPoint (Vector3 point)
    {
        if (_points == null) 
            return false;

        _points.Add(point);
        return true;
    }
    public bool RemoveLastPoint()
    {
        if (_points == null || _points.Count < 1) 
            return false;

        _points.RemoveAt(_points.Count - 1);
        return true;
    }

    public Type GetToolType() => _tool.GetType();
    public List<Vector3> GetPoints() => _points;
    public List<IQuantity> GetQuantities() => _quantities;
}