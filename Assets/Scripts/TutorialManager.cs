/// <summary>
/// Use the EasyPopup library to create a popup to provide basic instructions when the game is opened.
/// </summary>

using EasyPopupSystem;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    public EasyPopup tutorialPopup;

    // Create information popup upon game start
    void Start()
    {
        // Only add popup in desktop mode, due to current limitations in mobile controls
        if (!(Application.isMobilePlatform  || UnityEngine.Device.Application.isMobilePlatform)) 
            CreateTutorialPopup();
    }

    void CreateTutorialPopup()
    {
        tutorialPopup = EasyPopup.Create("Welcome",
            "Navigate through the house and complete the tasks in each room.",
            "PopupInfo",
            null,
            null,
            false,
            "Press enter to continue"
        );
    }

    // Toggle popup from keyboard input
    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame && tutorialPopup != null)
        {
            tutorialPopup.Hide();
        } else if (Keyboard.current.hKey.wasPressedThisFrame && tutorialPopup == null)
        {
            CreateTutorialPopup();
        }
    }
}