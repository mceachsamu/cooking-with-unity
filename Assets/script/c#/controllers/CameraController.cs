using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //positions holds tell the controller the different positions it can move to
    //each object in this array should have a CameraTag script attached
    public GameObject[] positions;

    //the animation speed of the camera
    public AnimationCurve cameraMoveSpeed;

    //the number of seconds it takes to transition through animation
    public float AnimationTimeInSeconds = 3;

    //the position the camera is starting at
    private Vector3 startPosition;

    //the position the camera is moving to/ is currently at
    private Vector3 targetPosition;


    //the rotation the camera is starting at
    private Quaternion startRotation;

    //the rotation the camera is moving to/ is currently at
    private Quaternion targetRotation;


    //the time the current animation started at
    private float startTime;

    void Start()
    {
        if (positions.Length == 0)
        {
            print("need to have atleast one position object on camera");
        }

        // Initialize target position to the first position in the list
        startPosition = positions[0].transform.position;
        targetPosition = positions[0].transform.position;
        
        // Initialize target rotation to the first rotation in the list
        startRotation = positions[0].transform.rotation;
        targetRotation = positions[0].transform.rotation;
    }

    void Update()
    {
        //listen for new position requests and update targetPosition and startTime
        ListenToInput();

        //get the speed the camera should currently be moving at (if its changing positions)
        float curFrame = GetAnimationFrame(cameraMoveSpeed, startTime);

        //interpolate between its current position and its new position
        this.transform.position = TranslateToPosition(startPosition, targetPosition, curFrame);
        this.transform.rotation = TranslateToRotation(startRotation, targetRotation, curFrame);
    }

    private void ListenToInput()
    {
        for (int i = 0; i < positions.Length; i++) {
            if (Input.GetKeyDown(positions[i].GetComponent<cameraPosition>().KeyStroke))
            {
                if (positions[i] != null)
                {
                    startTime = Time.time;

                    startPosition = this.transform.position;
                    startRotation = this.transform.rotation;

                    targetRotation = positions[i].transform.rotation;
                    targetPosition = positions[i].transform.position;
                }
            }
        }
    }

    // Returns a float between 0.0 and 1.0, corresponding to the point in the animation we are on
    private float GetAnimationFrame(AnimationCurve speedCurve, float sTime) {
        float timeSinceStart = Time.time - sTime;
        float endTime = AnimationTimeInSeconds;
        float normalizedTime = timeSinceStart/endTime;

        //using normalized time will make the function go from 0 to 1 in AnimationTimeInSeconds;
        float speed = speedCurve.Evaluate(normalizedTime);
        return speed;
    }


    private Vector3 TranslateToPosition(Vector3 currentPosition, Vector3 newPosition, float frame)
    {
        Vector3 InterpolatedPos = currentPosition * (1.0f - frame) + newPosition * frame;
        return InterpolatedPos;
    }

    private Quaternion TranslateToRotation(Quaternion currentRotation, Quaternion newRotation, float frame)
    {
        Quaternion InterpolatedRot = Quaternion.Lerp(currentRotation, newRotation, frame);
        return InterpolatedRot;
    }
}
