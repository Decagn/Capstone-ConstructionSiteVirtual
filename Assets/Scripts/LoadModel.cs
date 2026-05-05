using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LoadModel : MonoBehaviour
{
    GameObject[] houseModels;

    void Start()
    {
        Camera mainCamera = Camera.main;

        houseModels = Resources.LoadAll<GameObject>("House Models/");

        // Display error message if no house models found
        if (houseModels.Length == 0)
        {
            GameObject canvasObj = GameObject.Find("Canvas");

            if (canvasObj != null)
            {
                Debug.Log("Error: no house models found in expected location");
            }

            return;
        }

        foreach (GameObject model in houseModels)
        {
            GameObject newModel = Instantiate<GameObject>(model, Vector3.zero, Quaternion.identity);

            // Add collisions to all GameObjects in model
            foreach (Transform child in newModel.transform)
            {
                child.AddComponent<BoxCollider>();
            }

            houseModels.Append(newModel);
        }

        // Disable all additional cameras within FBX models
        Camera[] allCameras = Camera.allCameras;

        if (allCameras != null)
        {
            foreach (Camera camera in allCameras)
            {
                camera.enabled = false;
            }
        }

        mainCamera.enabled = true;
    }
}
