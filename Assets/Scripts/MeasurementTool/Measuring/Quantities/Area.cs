using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/*
 * [Quantity] Area
 * The area of a polygon formed by an enclosed perimeter.
 */
public class Area : IQuantity
{
    public float Value { get; }
    public string Unit { get; }
    public int DecimalPlaces { get; }
    public QuantityType Type { get; }

    // list of points that represent the vertices of the polygon.
    private List<Vector3> points;

    public Area(List<Vector3> points)
    {
        this.Value = Calculator.GetAreaOfPolygon(points);
        this.Unit = "m²";
        this.DecimalPlaces = 2;
        this.Type = QuantityType.Area;
        this.points = points;
    }

    public List<Vector3> GetPoints => points;
}
