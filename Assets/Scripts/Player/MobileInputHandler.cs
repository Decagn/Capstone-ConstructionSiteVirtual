using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// MobileInputHandler manages all mobile input for the player, including:
/// - Joystick-based horizontal movement (forward, back, left, right)
/// - Vertical movement via on-screen Up and Down buttons
/// - Touch-based camera rotation using the right half of the screen
/// - Editor simulation of touch camera rotation using right mouse button drag
/// 
/// This script should be attached to the Player GameObject.
/// Assign the joystick, up button, and down button references in the Inspector.
/// </summary>
public class MobileInputHandler : MonoBehaviour
{
    [Header("Movement")]
    // Reference to the on-screen joystick component used for horizontal movement.
    public TouchJoystick joystick;

    // Movement speed in Unity units per second.
    public float moveSpeed = 5f;

    [Header("Look")]
    // Sensitivity multiplier for touch and mouse camera rotation.
    // Higher values = faster rotation response.
    public float lookSensitivity = 0.1f;

    // Tracks whether the Up button is currently being held down.
    private bool _moveUp = false;

    // Tracks whether the Down button is currently being held down.
    private bool _moveDown = false;

    // Reference to the CharacterController used to physically move the player.
    private CharacterController _cc;

    // Reference to the Camera's Transform for applying vertical (pitch) rotation.
    private Transform _cameraTransform;

    // Stores the current vertical rotation angle of the camera in degrees.
    // Clamped to prevent over-rotation past straight up or straight down.
    private float _xRotation = 0f;

    // Stores the touch ID of the finger currently controlling the camera look.
    // Used to track a specific finger across multiple touch frames.
    private int _lookTouchId = -1;

    // Stores the screen position of the look touch from the previous frame.
    // Used to calculate how much the finger has moved (delta) each frame.
    private Vector2 _lastLookPos;

    /// <summary>
    /// Called once when the script is first enabled.
    /// Retrieves required components from the Player GameObject.
    /// </summary>
    void Start()
    {
        _cc = GetComponent<CharacterController>();
        _cameraTransform = GetComponentInChildren<Camera>().transform;

        // Unlock the cursor for mobile mode so mouse input is not consumed by cursor lock.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Called once per frame.
    /// Delegates movement and look handling to separate methods for clarity.
    /// </summary>
    void Update()
    {
        HandleMove();
        HandleLook();
    }

    /// <summary>
    /// Moves the player based on joystick input and vertical button state.
    /// Joystick controls left/right and forward/back relative to the player's facing direction.
    /// Up and Down buttons control vertical movement along the world Y axis.
    /// </summary>
    void HandleMove()
    {
        if (joystick == null) return;

        // Read the current joystick input direction and magnitude.
        Vector2 input = joystick.Input;

        // Convert joystick input into a world-space movement vector
        // relative to the direction the player is currently facing.
        Vector3 move = transform.right * input.x + transform.forward * input.y;

        // Apply vertical movement based on which button is currently held.
        float verticalInput = 0f;
        if (_moveUp) verticalInput = 1f;
        if (_moveDown) verticalInput = -1f;

        // Add vertical movement along the world Y axis.
        move += Vector3.up * verticalInput;

        // Apply the final movement vector to the CharacterController.
        // Multiplying by moveSpeed and Time.deltaTime ensures frame-rate-independent movement.
        _cc.Move(move * moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Rotates the camera and player based on touch input from the right half of the screen.
    /// Horizontal touch movement rotates the player body (yaw).
    /// Vertical touch movement rotates the camera up and down (pitch), clamped to prevent flipping.
    /// This method is designed for real touchscreen devices only.
    /// </summary>
    void HandleLook()
    {
        // Read all current touch inputs from the touchscreen device.
        var touches = Touchscreen.current?.touches;
        if (touches == null) return;

        foreach (var touch in touches)
        {
            var phase = touch.phase.ReadValue();
            Vector2 pos = touch.position.ReadValue();

            // Only process touches on the right half of the screen.
            // The left half is reserved for the joystick movement input.
            if (pos.x < Screen.width / 2f) continue;

            if (phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                // Record which finger is controlling the camera and its starting position.
                _lookTouchId = touch.touchId.ReadValue();
                _lastLookPos = pos;
            }
            else if (phase == UnityEngine.InputSystem.TouchPhase.Moved
                     && touch.touchId.ReadValue() == _lookTouchId)
            {
                // Calculate how far the finger has moved since the last frame.
                Vector2 delta = pos - _lastLookPos;
                _lastLookPos = pos;

                float mouseX = delta.x * lookSensitivity;
                float mouseY = delta.y * lookSensitivity;

                // Apply vertical pitch to the camera only.
                _xRotation -= mouseY;
                _xRotation = Mathf.Clamp(_xRotation, -80f, 80f);
                _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

                // Apply horizontal yaw to the entire player body.
                transform.Rotate(Vector3.up * mouseX);
            }
            else if (phase == UnityEngine.InputSystem.TouchPhase.Ended
                     && touch.touchId.ReadValue() == _lookTouchId)
            {
                // Release the tracked finger when it lifts off the screen.
                _lookTouchId = -1;
            }
        }
    }

    /// <summary>
    /// Called by the UpButton's EventTrigger PointerDown event.
    /// Sets the flag to move the player upward each frame while the button is held.
    /// </summary>
    public void OnUpButtonDown()
    {
        _moveUp = true;
    }

    /// <summary>
    /// Called by the UpButton's EventTrigger PointerUp event.
    /// Clears the upward movement flag when the button is released.
    /// </summary>
    public void OnUpButtonUp()
    {
        _moveUp = false;
    }

    /// <summary>
    /// Called by the DownButton's EventTrigger PointerDown event.
    /// Sets the flag to move the player downward each frame while the button is held.
    /// </summary>
    public void OnDownButtonDown()
    {
        _moveDown = true;
    }

    /// <summary>
    /// Called by the DownButton's EventTrigger PointerUp event.
    /// Clears the downward movement flag when the button is released.
    /// </summary>
    public void OnDownButtonUp()
    {
        _moveDown = false;
    }
}