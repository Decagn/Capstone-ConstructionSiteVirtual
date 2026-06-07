/// <summary>
/// Loads and tracks all available house models, and manages display of model for current lesson.
/// </summary>

using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

public class ModelLoader : MonoBehaviour
{
    Dictionary<string, GameObject> houseModels;
    GameObject currentModel;

    void Start()
    {
        // Initialise empty dictionary
        Dictionary<string, GameObject> houseFiles = new Dictionary<string, GameObject>();

        // Load all house models found in expected directory and store in dict
        DirectoryInfo houseDir = new DirectoryInfo("Assets/Resources/HouseModels");
        foreach (var file in houseDir.EnumerateFiles()
            .Where(file => file.Extension == ".fbx" || file.Extension == ".skp"))
        {
            Debug.Log($"Attempting to load model at path: HouseModels/{file.Name}");
            GameObject newModel = Resources.Load<GameObject>($"HouseModels/{Path.GetFileNameWithoutExtension(file.Name)}");
            Debug.Log($"New model loaded: {newModel}");
            houseFiles.Add(Path.GetFileNameWithoutExtension(file.Name), newModel);
        }

        // Display warning message and return if no house models found
        if (houseFiles.Count == 0)
        {
            Debug.LogError("[ModelLoader] Error: no house files located, exiting");
            return;
        }

        // Instantiate, disable and store all available house models
        houseModels = new Dictionary<string, GameObject>();
        foreach (var model in houseFiles)
        {
            GameObject newModel = Instantiate<GameObject>(model.Value, new Vector3(0, 1, 0), Quaternion.identity);
            newModel.SetActive(false);

            // Add collisions to model GameObjects, excluding doors
            foreach (Transform child in newModel.transform.GetComponentsInChildren<Transform>()
                .Where(child => !child.name.Contains("Door")))
            {
                child.gameObject.AddComponent<MeshCollider>();
            }

            houseModels.Add(model.Key, newModel);
        }
    }

    public void DisplayModel(string modelStr, List<InteractiveObject> interactiveObjects=null)
    {
        // If there is a model currently being displayed, disable it
        currentModel?.SetActive(false);

        // Locate GameObject for model selected in lesson plan
        if (!houseModels.TryGetValue(modelStr, out currentModel))
        {
            // If no available model with requested name, log warning and return
            Debug.LogError($"[ModelLoader] Error: house model with name {modelStr} not found");
            return;
        }

        // Build dictionary of interactive objects within current level
        Dictionary<string, InteractiveObject> interactiveAllocs = new Dictionary<string, InteractiveObject>();
        foreach(InteractiveObject interactiveObject in interactiveObjects)
        {
            interactiveAllocs.Add(interactiveObject.gameObject, interactiveObject);
        }

        // Enable selected model
        currentModel.SetActive(true);

        foreach (Transform child in currentModel.transform)
        {
            // Disable all extra cameras within model
            Camera extraCam = child.GetComponent<Camera>();

            if (extraCam != null)
            {
                extraCam.enabled = false;
            }

            // Add the correct BuildingElement component to specific GameObjects
            // This is required for them to be part of the task system
            if (interactiveAllocs.ContainsKey(child.gameObject.name))
            {
                InteractiveObject elementConfig = interactiveAllocs[child.gameObject.name];
                BuildingElement newElement = child.gameObject.AddComponent<BuildingElement>();        
                newElement.elementName = elementConfig.elementName;
                newElement.material = elementConfig.material;
                newElement.description = elementConfig.description;
                newElement.elementId = elementConfig.elementId;
            }
        }
    }
};
