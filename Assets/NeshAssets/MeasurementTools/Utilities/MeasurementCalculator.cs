using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class MeasurementType 
{
    public float value;
}

public class Length : MeasurementType
{
    public Vector3 pointA { get; private set; }
    public Vector3 pointB { get; private set; }

    public Length(Vector3 pointA, Vector3 pointB)
    {
        this.pointA = pointA;
        this.pointB = pointB;
        this.value = Calculator.MeasureLength(pointA, pointB);
    }
    public float Calc(Vector3 pointA, Vector3 pointB) => Vector3.Distance(pointA, pointB);
}

public class Angle : MeasurementType    
{
    public Vector3 pointA { get; private set; }
    public Vector3 vertex { get; private set; }
    public Vector3 pointB { get; private set; }
    public Angle(Vector3 pointA, Vector3 vertex, Vector3 pointB)
    {
        this.pointA = pointA;
        this.pointB = pointB;
        this.vertex = vertex;
        this.value = Calculator.MeasureAngle(pointA, vertex, pointB);
    }
}

public static class Calculator
{ 
    public static float MeasureLength(Vector3 pointA, Vector3 pointB) => Vector3.Distance(pointA, pointB);

    public static float MeasureAngle(Vector3 pointA, Vector3 vertex, Vector3 pointB)
    {
        Vector3 lineA = (pointA - vertex).normalized;
        Vector3 lineB = (pointB - vertex).normalized;
        return Vector3.Angle(lineA, lineB);
    }
}

