using UnityEngine;

/*
 *  IQuantity interface
 *  All quatities (length, angle, area, etc.) must implement this interface.
 */
public interface IQuantity
{
    float Value { get; }
    string Unit { get; }
    int DecimalPlaces { get; }
    QuantityType Type { get; }
}

public enum QuantityType
{ 
    Length,
    Angle,
    Area
}

