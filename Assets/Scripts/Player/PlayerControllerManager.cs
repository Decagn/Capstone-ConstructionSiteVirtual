using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This code manages the player controllers and cameras.
/// Currently experimental and does not work with existing systems as is, 
/// because need to understand how Mike's code works first, if we want to use this anyway.
public class PlayerControllerManager : MonoBehaviour
{
    [Header("Player Controllers")]
    public MonoBehaviour playerController1;
    public MonoBehaviour playerController2;

    [Header("Cameras")]
    public GameObject camera1;
    public GameObject camera2;

    private int activePlayer = 1;

    public void Start()
    {
        SetActivePlayer(1);
    }

    public void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            // Switches then sets the active Player if key is pressed
            activePlayer = activePlayer == 1 ? 2 : 1;
            SetActivePlayer(activePlayer);
        }
    }

    public void SetActivePlayer(int playerNum)
    {
        playerController1.enabled = false;
        playerController2.enabled = false;
        camera1.SetActive(false);
        camera2.SetActive(false);

        switch (playerNum)
        {
            case 1:
            playerController1.enabled = true;
            camera1.SetActive(true);
            break;

            case 2:
            playerController2.enabled = true;
            camera2.SetActive(true);
            break;

        }
    }
}
