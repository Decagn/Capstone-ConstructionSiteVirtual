using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public static class PointSelector
{
    /*
     * [SYSTEM DESIGN]
     * Max ray length determines how far in the scene can points be selected.
     * Adjust to a distance that makes sense in terms of how far we should be able to measure.
     */
    private const float _maxRayLength = Mathf.Infinity;

    public static Vector3 TrySelectPoint(Vector2 pointOnScreen)
    {
        Ray ray = Camera.main.ScreenPointToRay(pointOnScreen);
        bool surfaceFound = Physics.Raycast(ray, out RaycastHit hit, _maxRayLength);
        return hit.point;
    }
}
