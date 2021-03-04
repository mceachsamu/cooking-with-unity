using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static ObjectFind;

public class UIController : MonoBehaviour
{
    private CameraController cameraController;

    private CharacterController characterController;

    public Canvas WindowUI;


    void Start()
    {
        GameObject camera = ObjectFind.FindFirstWithTag("MainCamera");
        cameraController = camera.GetComponent<CameraController>();

        GameObject characterControl = ObjectFind.FindFirstWithTag("CharacterController");
        characterController = characterControl.GetComponent<CharacterController>();

    }

    void Update()
    {
        CameraController.CameraPosition camPos = cameraController.GetCameraPosition();

        switch (camPos)
        {
            case CameraController.CameraPosition.KITCHEN:
                WindowUI.enabled = false;
                break;
            case CameraController.CameraPosition.POTION:
                WindowUI.enabled = false;
                break;
            case CameraController.CameraPosition.WINDOW:
                WindowUI.enabled = WindowUIEnabled();
                break;
        }

    }

    private bool WindowUIEnabled() {
        return characterController.CharacterCurrentlyOrdering();
    }


}
