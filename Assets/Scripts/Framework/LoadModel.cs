using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LoadModel : MonoBehaviour
{
    List<GameObject> houseModels;

    public List<GameObject> loadModel()
    {
        houseModels = new List<GameObject>();
        GameObject[] houseFiles = Resources.LoadAll<GameObject>("House Models/");

        // Display error message if no house models found
        if (houseFiles.Length == 0)
        {
            Debug.Log("Error: no house models found in expected location");
            return houseModels;
        }

        foreach (GameObject model in houseFiles)
        {
            GameObject newModel = Instantiate<GameObject>(model, Vector3.zero, Quaternion.identity);
            newModel.SetActive(false);

            // Add collisions to all GameObjects in model
            foreach (Transform child in newModel.transform)
            {
                child.AddComponent<MeshCollider>();
            }

            houseModels.Add(newModel);
        }

        return houseModels;
    }

    private void EnableModel(GameObject model)
    {
        model.SetActive(true);

        // Disable all extra cameras within model
        foreach (Transform child in model.transform)
        {
            Camera extraCam = child.GetComponent<Camera>();

            if (extraCam != null)
            {
                extraCam.enabled = false;
            }
        }
    }

   public GameObject DisplayModel(string modelName)
    {
        // Enable correct model
        GameObject currentModel = houseModels.Where(model => model.name == modelName).SingleOrDefault();

        if (currentModel != null)
        {
            currentModel.SetActive(true);

            EnableModel(currentModel);
        } else
        {
            Debug.Log($"Selected model {modelName} not found");
        }

        return currentModel;
    }
}
