using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBehaviour : MonoBehaviour
{

    public float startTime;
    public float endTime;

    //placeholder
    public GameObject moveTo;

    protected bool isOrdering = false;


    //returns the window of time the character will show up in
    public Vector2 GetTimeWindow()
    {
        return new Vector2(startTime, endTime);
    }

    public abstract CharacterPhase GetCharacterPhase();

    //determine the potion order the character will make at this point in the game
    public abstract Order GetCharacterOrder();

    public bool IsCurrentlyOrdering()
    {
        return isOrdering;
    }

    public abstract void SetCharacterOrdering();

    public bool CanArriveToOrder(float currentTime)
    {
        return currentTime > startTime && currentTime < endTime;
    }

    public abstract string GetCharacterText();

}
