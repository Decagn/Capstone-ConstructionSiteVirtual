using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/*
 * Lesson Measurements File Parser
 * This class finds the pre-defined/lesson measurement points within a file in /Resources
 * and is used by the Lesson Measurements Manager.
 */
public class LessonMeasurementsFileParser
{
    // Tries to convert a string into a Vector3,
    // returns null if unable to.
    private static Vector3? ParseStringForVector3(string vectorString)
    {
        string[] components = vectorString.Split(',');

        // A Vector3 must have 3 components: x, y, z.
        if (components.Length != 3) 
            return null;

        if (float.TryParse(components[0], out float x) &&
            float.TryParse(components[1], out float y) &&
            float.TryParse(components[2], out float z))
        {
            return new Vector3(x, y, z);
        }

        return null;
    }

    // Parses a single line for all the Vector3s in that line.
    // One line represents one measurement (i.e. the perimeter of a kitchen counter).
    private static List<Vector3> ParseLineForVector3s(string line)
    {
        List<Vector3> measurement = new List<Vector3>();

        line = line.Replace(" ", "");
        string[] vectorStrings = line.Split(new string[] { "),(", ")" }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string s in vectorStrings)
        {
            string cleaned = s.TrimStart('(');
            Vector3? point = ParseStringForVector3(cleaned);

            if (point == null) 
                return null;

            measurement.Add(point.Value);
        }

        // Add first point to the end of the line to draw a connected polygon
        // this ensures all sides of the shape has a line drawn.
        measurement.Add(measurement[0]);

        return measurement;
    }

    // Parses the lesson measurement file line by line
    // and extracts all the measurements from it.
    private static List<List<Vector3>> ParseFileForMeasurements(string[] lines)
    {
        List<List<Vector3>> lessonMeasurements = new List<List<Vector3>>();

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            // skip comments and empty lines.
            if (string.IsNullOrEmpty(trimmedLine) || line.StartsWith("#")) 
                continue;

            List<Vector3> measurement = ParseLineForVector3s(trimmedLine);

            if (measurement != null) 
                lessonMeasurements.Add(measurement);
        }

        return lessonMeasurements;
    }

    // returns a list of lesson measurements from the file passed in.
    public static List<List<Vector3>> GetMeasurements(string filename)
    {
        TextAsset file = Resources.Load<TextAsset>(filename);

        if (file == null)
        {
            DebugMeasurementTools.Log("Lesson Measurements file could not be found", "LessonMeasurementsFileParser");
            return new List<List<Vector3>>();
        }

        string[] lines= file.text.Split('\n');
        return ParseFileForMeasurements(lines);
    }
}
