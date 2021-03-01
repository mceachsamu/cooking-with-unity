using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBehaviour
{

    public float startTime;
    public float endTime;
    private bool isOrdering;


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

    }

    public void SetCharacterOrdering(bool ordering)
    {
        isOrdering = ordering;
    }

    public abstract bool CanArriveToOrder();

}
