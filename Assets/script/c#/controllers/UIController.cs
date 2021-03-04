using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static ObjectFind;

public class UIController : MonoBehaviour
{
    private CameraController cameraController;

    public Canvas WindowUI;


    void Start()
    {
        GameObject camera = ObjectFind.FindFirstWithTag("MainCamera");
        cameraController = camera.GetComponent<CameraController>();

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
                WindowUI.enabled = true;
                break;
        }
    }


}
