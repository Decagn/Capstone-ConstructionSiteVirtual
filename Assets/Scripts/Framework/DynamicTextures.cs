/// <summary>
/// Temporary utility class to be used to dynamically add textures to models.
/// IMPORTANT: Any texture used must be placed within Resources/DynamicTextures with read/write enabled.
/// </summary>

using System.Collections.Generic;
using UnityEngine;

public class DynamicTextures : MonoBehaviour
{
    // Dict tracking which textures should be dynamically allocated to which GameObjects within models
    private Dictionary<string, string> _textureAlloc;

    void Start()
    {
        _textureAlloc = new Dictionary<string, string>()
        {
            { "Island Bench Kitchen - Island Bench 2500mm [238699]", "Marble016" }
        };
    }

    // To be called by master LessonLoader, adds specified textures to GameObjects within model
    public void AddTextures()
    {
        foreach(var item in _textureAlloc)
        {
            GameObject targetObj = GameObject.Find(item.Key);

            // Specified GameObject may not be present in current model
            if (targetObj == null)
            {
                continue;
            }

            Debug.Log($"Adding texture '{item.Value}' to GameObject '{item.Key}'");

            Texture2D targetTexture = Resources.Load<Texture2D>($"DynamicTextures/{item.Value}");

            if (targetTexture == null)
            {
                Debug.LogError($"Texture {item.Value} not found, exiting...");
                continue;
            }

            Renderer targetRenderer = targetObj.GetComponent<Renderer>();

            targetTexture.Apply();
            targetRenderer.material.mainTexture = targetTexture;
        }
    }
}
