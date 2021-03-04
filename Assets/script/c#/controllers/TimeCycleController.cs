using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCycleController : MonoBehaviour
{

    //a multipler to speed up/slow down day
    public float speedMultiplier = 1.0f;

    //the length of the game day
    private static float dayLength = 24.0f;

    private float startTime;

    private float timeCount;

    void Start()
    {
    }


    void Update()
    {
        if ((timeCount - startTime) * speedMultiplier > dayLength)
        {
            //reset timer
            startTime = 0.0f;
            timeCount = 0.0f;
        }
        timeCount++;
    }

    //returns a time value between 0.0 (day start) and 1.0 (day end)
    public float GetTime() {
        //the value goes a little over 1.0, so just put a clamp on
        return Mathf.Clamp(((timeCount - startTime) * speedMultiplier),0.0f,dayLength);
    }


}
