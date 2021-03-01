using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectFind;

public class CharacterController : MonoBehaviour
{
    private TimeCycleController timeController;

    private CharacterBehaviour[] characters;


    void Start()
    {
        GameObject timer = ObjectFind.FindFirstWithTag("TimeController");
        timeController = timer.GetComponent<TimeCycleController>();


        GameObject[] characterObjects = ObjectFind.FindAllWithTag("Character");
        characters = new CharacterBehaviour[characterObjects.Length];
        for (int i = 0; i < characterObjects.Length; i++)
        {
            characters[i] = characterObjects[i].GetComponent<CharacterBehaviour>();
        }
    }

    void Update()
    {
        if (CanCharacterArrive())
        {

            //need to do this step at random frames
            CharacterBehaviour currentCharacter = SelectNextCharacterToOrder();
            if (currentCharacter != null)
            {
                currentCharacter.SetCharacterOrdering(true);
            }
        }
    }

    //returns true if no characters are currently ordering
    private bool CanCharacterArrive()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            bool canArrive = characters[i].IsCurrentlyOrdering();
            if (!canArrive)
            {
                return false;
            }
        }

        return true;
    }

    private CharacterBehaviour SelectNextCharacterToOrder()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i].CanArriveToOrder())
            {
                return characters[i];
            }
        }
        return null;
    }

}
