using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// FirstPersonController handles all desktop player movement and camera look controls.
/// 
/// Movement: WASD keys for horizontal movement, E/Q keys for vertical movement (fly mode).
/// Look: Mouse delta input controls camera pitch (up/down) and player yaw (left/right).
/// 
/// This script requires a CharacterController component on the same GameObject.
/// Attach the player's Camera transform to the cameraTransform field in the Inspector.
/// This script uses Unity's new Input System package.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    // The speed at which the player moves through the scene, measured in Unity units per second.
    public float moveSpeed = 5f;

    [Header("Look")]
    // Controls how fast the camera rotates in response to mouse movement.
    // Higher values = more sensitive, lower values = smoother.
    public float mouseSensitivity = 2f;

    // Reference to the Camera's Transform, used to control vertical (pitch) rotation.
    // This should be the Camera child object of the Player GameObject.
    public Transform cameraTransform;

    // Tracks the current vertical rotation (pitch) of the camera in degrees.
    // Clamped to prevent the player from rotating past straight up or straight down.
    private float _xRotation = 0f;

    private float verticalInput = 0f;

    /// <summary>
    /// Called once when the script is first enabled, before the first Update.
    /// Retrieves the CharacterController component and locks the cursor to the center
    /// of the screen so the player can look around freely without the cursor leaving the window.
    /// </summary>
    void Start()
    {
        // Lock the cursor to the center of the Game window for mouse-look to work correctly.
        Cursor.lockState = CursorLockMode.Locked;

        // Hide the cursor while the game is running so it does not appear on screen.
        Cursor.visible = false;
    }

    /// <summary>
    /// Called once per frame by Unity.
    /// Delegates to HandleLook() and HandleMove() each frame to keep logic separated.
    /// </summary>
    void Update()
    {
        HandleLook();
        HandleMove();
    }

    /// <summary>
    /// Reads mouse delta input and applies rotation to the player and camera.
    /// Horizontal mouse movement (X axis) rotates the entire Player GameObject left/right (yaw).
    /// Vertical mouse movement (Y axis) rotates only the Camera up/down (pitch),
    /// clamped between -80 and 80 degrees to prevent over-rotation.
    /// </summary>
    void HandleLook()
    {
        // Read how much the mouse has moved since the last frame.
        // Mouse.current.delta gives the raw pixel movement of the mouse.
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // Scale the mouse movement by sensitivity and a smoothing factor (0.1f).
        // The 0.1f factor converts raw pixel delta into a usable rotation amount.
        float mouseX = mouseDelta.x * mouseSensitivity * 0.1f;
        float mouseY = mouseDelta.y * mouseSensitivity * 0.1f;

        // Subtract mouseY from _xRotation because moving the mouse up should
        // rotate the camera upward (negative X rotation in Unity's coordinate system).
        _xRotation -= mouseY;

        // Clamp the vertical rotation so the player cannot look further than
        // 80 degrees up or 80 degrees down, preventing a full vertical flip.
        _xRotation = Mathf.Clamp(_xRotation, -89f, 89f);

        // Apply the clamped vertical rotation to the Camera transform only.
        // Using localRotation keeps the camera rotation relative to the player body.
        cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        // Rotate the entire Player GameObject horizontally around the world Y axis.
        // This turns the player left/right, which also turns the camera with it.
        transform.Rotate(Vector3.up * mouseX);
    }

    /// <summary>
    /// Reads keyboard input and moves the player accordingly.
    /// WASD controls horizontal movement relative to the direction the player is facing.
    /// E and Q control vertical movement (fly up / fly down) along the world Y axis.
    /// No gravity is applied in this mode, allowing free-fly navigation of the scene.
    /// </summary>
    void HandleMove()
    {
        // Accumulate horizontal movement input from WASD keys.
        // moveInput.x = left/right (A/D), moveInput.y = forward/back (W/S).
        Vector2 moveInput = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
        if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
        if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
        if (Keyboard.current.dKey.isPressed) moveInput.x += 1;

        // Read vertical movement input from E (up) and Q (down) keys.
        // This allows the player to fly up and down freely, which is useful
        // for inspecting construction elements at different heights.
        verticalInput = 0f;
        if (Keyboard.current.spaceKey.isPressed) verticalInput += 1f;
        if (Keyboard.current.ctrlKey.isPressed) verticalInput -= 1f;

        // Combine all three movement directions into a single world-space vector.
        // transform.right and transform.forward are relative to the player's current rotation,
        // so movement always feels correct regardless of which way the player is facing.
        // Vector3.up is always the world Y axis, keeping vertical movement absolute.
        Vector3 move = transform.right * moveInput.x
                     + transform.forward * moveInput.y
                     + transform.up * verticalInput;

        // Apply the movement to the CharacterController.
        // Multiplying by moveSpeed and Time.deltaTime ensures consistent speed
        // regardless of the current frame rate.
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }
}
