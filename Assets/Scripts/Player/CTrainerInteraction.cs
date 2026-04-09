using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// CTrainerInteraction handles the player's ability to interact with
/// building elements in the scene by casting a ray from the center of
/// the screen when the left mouse button is pressed.
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
    /// Checks if the player has pressed the left mouse button and triggers
    /// the interaction raycast if so.
    /// </summary>
    void Update()
    {
        // Use the new Input System to detect a left mouse button press.
        // wasPressedThisFrame ensures the interaction only fires once per click,
        // not continuously while the button is held down.
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryInteract();
        }
    }

    /// <summary>
    /// Casts a ray from the center of the screen into the scene.
    /// If the ray hits an interactable object, it logs the object's name
    /// and calls OnClick() on its BuildingElement component if one exists.
    /// </summary>
    void TryInteract()
    {
        // Build a ray originating from the exact center of the screen.
        // This simulates a crosshair-based interaction system where the player
        // aims by moving the camera rather than moving the mouse cursor.
        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        // Perform the raycast. Only objects on the interactableLayer will register a hit.
        // The hit information (position, normal, object reference, etc.) is stored in 'hit'.
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            // Log the name of the hit object for debugging purposes.
            Debug.Log($"[Interaction] Hit: {hit.collider.gameObject.name}");

            // Check if the hit object has a BuildingElement component attached.
            // BuildingElement stores metadata about structural components (name, material, description).
            var element = hit.collider.GetComponent<BuildingElement>();

            // If a BuildingElement component is found, trigger its OnClick behaviour.
            // This will display the element's information (currently via Debug.Log,
            // and later via a UI panel).
            if (element != null)
                element.OnClick();
        }
    }
}
