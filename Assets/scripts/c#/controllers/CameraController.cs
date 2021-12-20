using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Positions holds tell the controller the different positions it can move to
    // Each object in this array should have a CameraTag script attached
    public GameObject[] Positions;

    // The animation speed of the camera
    public AnimationCurve CameraMoveSpeed;

    // The number of seconds it takes to transition through animation
    public float AnimationTimeInSeconds = 3;

    // The position the camera is starting at
    private Vector3 _startPosition;

    // The position the camera is moving to/ is currently at
    private Vector3 _targetPosition;

    // The rotation the camera is starting at
    private Quaternion _startRotation;

    // The rotation the camera is moving to/ is currently at
    private Quaternion _targetRotation;

    // The time the current animation started at
    private float _startTime;

    void Start()
    {
        if (Positions.Length == 0)
        {
            print("need to have atleast one position object on camera");
        }

        // Initialize target position to the first position in the list
        _startPosition = Positions[0].transform.position;
        _targetPosition = Positions[0].transform.position;
        
        // Initialize target rotation to the first rotation in the list
        _startRotation = Positions[0].transform.rotation;
        _targetRotation = Positions[0].transform.rotation;
    }

    void Update()
    {
        // Listen for new position requests and update targetPosition and startTime
        ListenToInput();

        // Get the speed the camera should currently be moving at (if its changing positions)
        float curFrame = GetAnimationFrame(CameraMoveSpeed, _startTime);

        // Interpolate between its current position and its new position
        this.transform.position = TranslateToPosition(_startPosition, _targetPosition, curFrame);
        this.transform.rotation = TranslateToRotation(_startRotation, _targetRotation, curFrame);
    }

    private void ListenToInput()
    {
        for (int i = 0; i < Positions.Length; i++) {
            if (Input.GetKeyDown(Positions[i].GetComponent<CameraPosition>().KeyStroke))
            {
                if (Positions[i] != null)
                {
                    _startTime = Time.time;

                    _startPosition = this.transform.position;
                    _startRotation = this.transform.rotation;

                    _targetRotation = Positions[i].transform.rotation;
                    _targetPosition = Positions[i].transform.position;
                }
            }
        }
    }

    // Returns a float between 0.0 and 1.0, corresponding to the point in the animation we are on
    private float GetAnimationFrame(AnimationCurve speedCurve, float sTime) {
        float timeSinceStart = Time.time - sTime;
        float endTime = AnimationTimeInSeconds;
        float normalizedTime = timeSinceStart/endTime;

        // Using normalized time will make the function go from 0 to 1 in AnimationTimeInSeconds;
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
