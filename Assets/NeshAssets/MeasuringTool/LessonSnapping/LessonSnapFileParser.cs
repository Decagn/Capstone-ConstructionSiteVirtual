using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LessonSnapFileParser
{
    private static Vector3? ParseVector3(string vectorString)
    {
        string[] components = vectorString.Split(',');

        if (components.Length != 3) return null;

        if (float.TryParse(components[0], out float x) &&
            float.TryParse(components[1], out float y) &&
            float.TryParse(components[2], out float z))
        {
            return new Vector3(x, y, z);
        }

        return null;
    }
    private static List<Vector3> ParseLine(string line)
    {
        List<Vector3> points = new List<Vector3>();

        line = line.Replace(" ", "");
        string[] vectorStrings = line.Split(new string[] { "),(", ")" }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string s in vectorStrings)
        {
            string cleaned = s.TrimStart('(');
            Vector3? point = ParseVector3(cleaned);

            if (point == null) return null;

            points.Add(point.Value);
        }

        /* Add first point to the end of the line to draw a connected polygon */
        points.Add(points[0]);

        return points;
    }
    private static List<List<Vector3>> ParseFile(string[] lines)
    {
        List<List<Vector3>> snapObjects = new List<List<Vector3>>();

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            if (string.IsNullOrEmpty(trimmedLine) || line.StartsWith("#")) continue;

            List<Vector3> objectVertices = ParseLine(trimmedLine);

            if (objectVertices != null) snapObjects.Add(objectVertices);
        }

        return snapObjects;
    }
    public static List<List<Vector3>> GetVertices(string filename)
    {
        TextAsset file = Resources.Load<TextAsset>(filename);

        if (file == null )
        {
            MeasDebug.Log("Lesson snap file could not be loaded", "LessonSnapFileParser");
            return new List<List<Vector3>>();
        }

        string[] lines= file.text.Split('\n');
        return ParseFile(lines);
    }
}
