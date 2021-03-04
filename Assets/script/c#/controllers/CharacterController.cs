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

        if (characterObjects.Length == 0)
        {
            print("no characters");
        }

        characters = new CharacterBehaviour[characterObjects.Length];
        for (int i = 0; i < characterObjects.Length; i++)
        {
            CharacterBehaviour b = characterObjects[i].GetComponent<CharacterBehaviour>();
            if (b != null)
            {
                characters[i] = b;
            }
        }
    }

    void Update()
    {
        if (CanCharacterArrive())
        {

            //need to do this step at random frames
            float rand = Random.Range(0.0f, 1.0f);
            if (rand < 0.1f)
            {

                CharacterBehaviour currentCharacter = SelectNextCharacterToOrder();
                if (currentCharacter != null)
                {
                    //its the responsibility of the chacter to set this back to false
                    currentCharacter.SetCharacterOrdering();

                }
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
            if (characters[i].CanArriveToOrder(timeController.GetTime()))
            {
                return characters[i];
            }
        }
        return null;
    }

}
