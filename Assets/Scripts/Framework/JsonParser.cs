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
    public List<InteractiveObject> interactiveObjects;
    public LessonConfig lessonConfig;

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
    public float fontSize;

    public static TextLabel CreateFromJSON(string jsonStr)
    {
        return JsonUtility.FromJson<TextLabel>(jsonStr);
    }
};

// Class holding the BuildingElement configuration to be attached to a specified GameObject
[System.Serializable]
public class InteractiveObject
{
    public string gameObject;
    public string elementName;
    public string material;
    public string description;
    public string elementId;

    public static InteractiveObject CreateFromJSON(string jsonStr)
    {
        return JsonUtility.FromJson<InteractiveObject>(jsonStr);
    }
};