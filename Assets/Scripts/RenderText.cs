using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RenderText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TextAsset txtData = (TextAsset)Resources.Load("MyText");
        LessonLabels labels = LessonLabels.CreateFromJSON(txtData.text);
        TextMesh textObj = GetComponent<TextMesh>();
        Debug.Log($"Model file: {labels.model}");

        foreach (TextLabel label in labels.labels)
        {
            Debug.Log($"label pos: {label.xyz_pos}, label text: {label.text}");
        }
    }
}

[System.Serializable]
public class LessonLabels
{
    public string model;
    public List<TextLabel> labels;

    public static LessonLabels CreateFromJSON(string jsonStr)
    {
        return JsonUtility.FromJson<LessonLabels>(jsonStr);
    }
}

[System.Serializable]
public class TextLabel
{
    public Vector3 xyz_pos;
    public string text;

    public static TextLabel CreateFromJSON(string jsonStr)
    {
        return JsonUtility.FromJson<TextLabel>(jsonStr);
    }
}