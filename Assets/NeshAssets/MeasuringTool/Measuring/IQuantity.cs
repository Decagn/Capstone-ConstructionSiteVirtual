using UnityEngine;

/*
 *  IQuantity interface
 *  All quatities (length, angle, etc.) must implement this interface.
 */
public interface IQuantity
{
    float Value { get; }
    string Unit { get; }
    int DecimalPlaces { get; }
}
