using UnityEngine;

public class RenderText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TextAsset txtData = (TextAsset)Resources.Load("MyText");
        TextInfo txtInfo = TextInfo.CreateFromJSON(txtData.text);
        TextMesh textObj = GetComponent <TextMesh>();
        textObj.text = txtInfo.text + "(" + txtInfo.len + ")";
    }
}

[System.Serializable]
public class TextInfo
{
    public string text;
    public int len;

    public static TextInfo CreateFromJSON(string jsonStr)
    {
        return JsonUtility.FromJson<TextInfo>(jsonStr);
    }
}