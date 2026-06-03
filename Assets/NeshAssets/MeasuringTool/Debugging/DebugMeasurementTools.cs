using System.Reflection;
using UnityEngine;

/*
 * Debug Measurement Tools
 * A static class to print debug messages to the console during development
 * and differentiate these logs from debug messages from other assets.
 */
public static class DebugMeasurementTools
{
    // Log takes in the message: what you want to print
    // and the feature: the script that is printing the message.
    public static void Log(string message, string feature)
    {
        string asset = "MeasuringTool";
        Debug.Log($"[ {asset} ][ {feature} ]: {message}");
    }
}
