using UnityEngine;

public struct Length : IQuantity
{
    public float Value { get; }
    public string Unit { get; }
    public int DecimalPlaces { get; }
    private Vector3 _pointA;
    private Vector3 _pointB;

    public Length(Vector3 pointA, Vector3 pointB)
    {
        this.Value = MeasCalc.GetLength(pointA, pointB);
        this.Unit = "m";
        this.DecimalPlaces = 2;
        _pointA = pointA;
        _pointB = pointB;
    }
}
