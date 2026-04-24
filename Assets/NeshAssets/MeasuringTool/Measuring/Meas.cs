using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using System;

/*
 * [SYSTEM DESIGN]
 * The IMeas interface allows for methods to return invalid Measurements
 * This is to check if the measurement taking was successful.
 */
public interface IMeas{}

public struct InvalidMeas : IMeas{}

public struct Meas : IMeas
{
    private IMeasTool _tool;
    private List<Vector3> _points;
    private List<IQuantity> _quantities;

    public Meas(IMeasTool tool, List<Vector3> points, IQuantity quantity)
    {
        _tool = tool;

        _points = new List<Vector3>();
        _points.AddRange(points);

        _quantities = new List<IQuantity>();
        _quantities.Add(quantity);
    }

    public bool AddQuantity(IQuantity quantity)
    {
        if (_quantities == null) return false;

        _quantities.Add(quantity);
        return true;
    }

    public bool RemoveLastQuantity()
    {
        if (_quantities == null || _quantities.Count < 1) return false;

        _quantities.RemoveAt(_quantities.Count - 1);
        return true;
    }

    public bool AddPoint (Vector3 point)
    {
        if (_points == null) return false;

        _points.Add(point);
        return true;
    }

    public bool RemoveLastPoint()
    {
        if (_points == null || _points.Count < 1) return false;

        _points.RemoveAt(_points.Count - 1);
        return true;
    }

    public Type GetToolType() => _tool.GetType();
    public List<Vector3> GetPoints() => _points;
    public List<IQuantity> GetQuantities() => _quantities;
}