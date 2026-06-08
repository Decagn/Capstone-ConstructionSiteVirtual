using UnityEngine;

/*
 * [Quantity] Length
 * The length between two points in 3D space.
 */
public struct Length : IQuantity
{
    public float Value { get; }
    public string Unit { get; }
    public int DecimalPlaces { get; }
    public QuantityType Type { get; }
    private Vector3 _pointA;
    private Vector3 _pointB;

    public Length(Vector3 pointA, Vector3 pointB)
    {
        this.Value = Calculator.GetLength(pointA, pointB);
        this.Unit = "m";
        this.DecimalPlaces = 2;
        this.Type = QuantityType.Length;
        _pointA = pointA;
        _pointB = pointB;
    }
}
