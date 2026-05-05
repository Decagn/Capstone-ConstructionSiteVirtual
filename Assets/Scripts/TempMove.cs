using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

// Temp code to test collisions for runtime-loaded house model

public class TempMove : MonoBehaviour
{
    InputAction moveAction;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        Vector2 moveVal = moveAction.ReadValue<Vector2>();

        if (moveVal != Vector2.zero)
        {
            this.transform.Translate(new Vector3(moveVal.x * Time.deltaTime, 0, moveVal.y * Time.deltaTime), Space.World);
        }
    }
}
