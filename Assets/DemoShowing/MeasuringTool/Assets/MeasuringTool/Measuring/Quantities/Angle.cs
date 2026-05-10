using UnityEngine;

public struct Angle : IQuantity
{
    public float Value { get; }
    public string Unit { get; }
    public int DecimalPlaces { get; }
    private Vector3 _pointA;
    private Vector3 _vertex;
    private Vector3 _pointB;

    public Angle(Vector3 pointA, Vector3 vertex, Vector3 pointB)
    {
        this.Value = MeasCalc.GetAngle(pointA, vertex, pointB);
        this.Unit = "°";
        this.DecimalPlaces = 1;
        _pointA = pointA;
        _vertex = vertex;
        _pointB = pointB;
    }
}