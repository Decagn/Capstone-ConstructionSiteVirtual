/// <summary>
/// Utility classes for parsing and organising information derived from lesson plan JSON files.
/// </summary>

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Interface for JSON parsing, returns LessonPlan class populated with JSON data
public class JsonParser : MonoBehaviour
{
    public LessonPlan ParseLesson(string jsonStr)
    {
        return LessonPlan.CreateFromJSON(jsonStr);
    }        
};

// Class to be populated through parsing of lesson plan JSON file
[System.Serializable]
public class LessonPlan
{
    public string modelName;
    public Vector3 playerPos;
    public List<TextLabel> labels;

    public static LessonPlan CreateFromJSON(string jsonStr)
    {
        return JsonUtility.FromJson<LessonPlan>(jsonStr);
    }
};

// Class to be populated as text label JSON data is parsed, implicitly called during LessonPlan construction
[System.Serializable]
public class TextLabel
{
    public Vector3 labelPos;
    public string text;

    public static TextLabel CreateFromJSON(string jsonStr)
    {
        return JsonUtility.FromJson<TextLabel>(jsonStr);
    }
};