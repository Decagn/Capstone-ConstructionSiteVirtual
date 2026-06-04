using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

/*
 * Scaler
 * This static class scales different classes of objects
 * to appropriate sizes as the player/camera moves further or closer to those objects.
 */
public static class Scaler
{
    // returns a multiplier that scales the object depending on its distance from the camera.
    private static float GetMultiplier(Camera cam, GameObject obj)
    {
        float distance = Vector3.Distance(cam.transform.position, obj.transform.position);

        // multiplier formula derived from quadratic regression using Desmos Graphing Calculator.
        double multiplier = -0.003f * Mathf.Pow(distance, 2) + 0.25 * distance - 0.15;

        // clamp multiplier so its not too big / small
        return Mathf.Clamp((float)multiplier, 0.5f, 2);
    }

    public static void ScaleObject(Camera cam, GameObject obj, float defaultSize)
    {
        float multiplier = GetMultiplier(cam, obj);
        float newSize = defaultSize * multiplier;

        obj.transform.localScale = new Vector3(newSize, newSize, newSize);
    }

    public static void ScaleLine(Camera cam, GameObject line, float defaultWidth)
    {
        float multiplier = GetMultiplier(cam, line);
        float newWidth = defaultWidth * multiplier;

        LineRenderer renderer = line.GetComponent<LineRenderer>();
        renderer.startWidth = newWidth;
        renderer.endWidth = newWidth;
    }

    public static void ScaleText(Camera cam, GameObject textObj, float defaultFontSize)
    {
        TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();
        float multiplier = GetMultiplier(cam, textObj);
        float newFontSize = defaultFontSize * multiplier;

        text.fontSize = newFontSize;
    }
}
