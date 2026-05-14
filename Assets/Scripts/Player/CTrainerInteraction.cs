using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// CTrainerInteraction handles the player's ability to interact with
/// building elements in the scene by casting a ray from the mouse position
/// when the right mouse button is pressed.
///
/// This script should be attached to the Player GameObject.
/// It requires a Camera reference and a LayerMask to filter interactable objects.
/// </summary>
public class CTrainerInteraction : MonoBehaviour
{
    [Header("Raycast")]
    // The maximum distance (in Unity units) the interaction ray will travel.
    // Objects beyond this range cannot be interacted with.
    public float interactRange = 20f;

    // Only objects on this layer will be detected by the raycast.
    // Set this to the "Interactable" layer in the Inspector.
    public LayerMask interactableLayer;

    // Reference to the player's camera, used as the origin point for the raycast.
    // Drag the Camera child object of the Player into this field in the Inspector.
    public Camera playerCamera;

    /// <summary>
    /// Called once per frame by Unity.
    /// Checks if the player has pressed the right mouse button and triggers
    /// the interaction raycast from the current mouse position.
    /// </summary>
    void Update()
    {
        // Skip interaction while the game is paused.
        if (Time.timeScale == 0f) return;

        // Use the new Input System to detect a right mouse button press.
        // wasPressedThisFrame ensures the interaction only fires once per click,
        // not continuously while the button is held down.
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            TryInteract();
        }
    }
    /// <summary>
    /// Casts a ray from the current mouse position into the scene.
    /// If the ray hits an interactable object, calls OnClick() on its
    /// BuildingElement component if one exists.
    /// </summary>
    public void TryInteract()
    {
        // On mobile, ray comes from screen centre.
        // On desktop, ray comes from current mouse position.
        Vector3 screenPoint;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        if (Mouse.current != null)
            screenPoint = Mouse.current.position.ReadValue();
        else
            screenPoint = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
#else
    screenPoint = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
#endif

        Ray ray = playerCamera.ScreenPointToRay(screenPoint);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            var element = hit.collider.GetComponent<BuildingElement>();
            Debug.Log(element != null);
            if (element != null)
                element.OnClick();
        }
    }
}