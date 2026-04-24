using UnityEngine;

public interface IQuantity
{
    float Value { get; }
    string Unit { get; }
    int DecimalPlaces { get; }
}
