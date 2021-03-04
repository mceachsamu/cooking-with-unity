using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wilson : CharacterBehaviour
{

    public override CharacterPhase GetCharacterPhase()
    {
        return CharacterPhase.REGULAR;
    }

    public override Order GetCharacterOrder()
    {
        return new Order(PotionType.HAIR_GROW, 1);
    }

    public override void SetCharacterOrdering()
    {
        this.isOrdering = true;
        this.transform.position = this.moveTo.transform.position;
    }
}
