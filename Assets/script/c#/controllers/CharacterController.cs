using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectFind;

public class CharacterController : MonoBehaviour
{
    private TimeCycleController _timeController;

    private CharacterBehaviour[] _characters;


    void Start()
    {
        GameObject timer = ObjectFind.FindFirstWithTag("TimeController");
        _timeController = timer.GetComponent<TimeCycleController>();


        GameObject[] characterObjects = ObjectFind.FindAllWithTag("Character");

        if (characterObjects.Length == 0)
        {
            print("no characters");
        }

        _characters = new CharacterBehaviour[characterObjects.Length];
        for (int i = 0; i < characterObjects.Length; i++)
        {
            CharacterBehaviour b = characterObjects[i].GetComponent<CharacterBehaviour>();
            if (b != null)
            {
                _characters[i] = b;
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
        for (int i = 0; i < _characters.Length; i++)
        {
            bool isOrdering = _characters[i].IsCurrentlyOrdering();
            if (isOrdering)
            {
                return false;
            }
        }

        return true;
    }

    private CharacterBehaviour SelectNextCharacterToOrder()
    {
        for (int i = 0; i < _characters.Length; i++)
        {
            if (_characters[i].CanArriveToOrder(_timeController.GetTime()))
            {
                return _characters[i];
            }
        }
        return null;
    }

    public bool CharacterCurrentlyOrdering()
    {
        for (int i = 0; i < _characters.Length; i++)
        {
            if (_characters[i].IsCurrentlyOrdering())
            {
                return true;
            }
        }
        return false;
    }
}
