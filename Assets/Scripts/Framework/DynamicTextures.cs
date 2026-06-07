/// <summary>
/// Temporary utility class to be used to dynamically add textures to models.
/// IMPORTANT: Any texture used must be placed within Resources/DynamicTextures with read/write enabled.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class DynamicTextures : MonoBehaviour
{
    // Dict tracking which textures should be dynamically allocated to which GameObjects within models
    private Dictionary<string, string> _textureAlloc;

    void Start()
    {
        _textureAlloc = new Dictionary<string, string>
        {
            { "Bench", "Marble016" },
            { "Roof", "Gravel043" },
            { "Brick", "Bricks097" },
            { "Wall Generic", "DarkPlaster05" },
            { "Wall Interior", "DarkPlaster05" },
            { "Wall Retaining", "Gravel043" },
            { "Bath", "Granite007A" },
            { "Ceiling", "DarkPlaster05" },
            { "Cook_Top", "Marble016" },
            { "Table", "WoodFloor043" },
            { "Dishwasher", "BeigeWall001" },
            { "Gutter", "Gravel043" },
            { "Floor", "WoodFloor043" },
            { "Stair", "WoodFloor043" },
            { "Riser", "WoodFloor043" },
            { "Level", "WoodFloor043" },
            { "Shelf", "WoodFloor043" },
            { "Shower", "Granite007A" },
            { "Sink", "Marble016" },
            { "Sofa", "PaintedPlaster016" },
            { "Window", "BlueGlass001" },
            { "House_Assessment", "WoodFloor051" }
        };
    }

    // Public interface for dynamic texture addition
    public void TextureAdder(string modelName)
    {
        StartCoroutine(AddTextureTask(modelName));
    }

    // Wait until texture dictionary construction is completed
    public IEnumerator AddTextureTask(string modelName)
    {
        yield return new WaitUntil(() => _textureAlloc != null);
        AddTextures(modelName);
    } 

    // Adds specified textures to GameObjects within model
    private void AddTextures(string modelName)
    {
        // `(Clone)` accounts for name change as part of resource loading
        GameObject houseModel = GameObject.Find($"{modelName}(Clone)");

        if (houseModel == null)
        {
            Debug.LogError($"[DynamicTextures] House model '{modelName}(Clone)' unable to be found");
            return;
        }

        foreach(var item in _textureAlloc)
        {
            List<GameObject> targetObjs = new List<GameObject>();

            // Casting to List enables Where method to be used
            foreach (Transform child in houseModel.transform.Cast<Transform>().ToList()
                .Where(child => child.name.Contains(item.Key)))
            {
                targetObjs.Add(child.gameObject);
            }

            // Specified GameObjects may not be present in current model
            if (targetObjs.Count() == 0)
            {
                continue;
            }

            Debug.Log($"Adding texture '{item.Value}' to GameObjects containing '{item.Key}'");

            Texture2D targetTexture = Resources.Load<Texture2D>($"DynamicTextures/{item.Value}");

            if (targetTexture == null)
            {
                Debug.LogError($"Texture {item.Value} not found, exiting...");
                continue;
            }

            foreach(GameObject targetObj in targetObjs)
            {
                Renderer targetRenderer = targetObj.GetComponent<Renderer>();
                targetTexture.Apply();
                targetRenderer.material.mainTexture = targetTexture;   
            }
        }
    }
}
