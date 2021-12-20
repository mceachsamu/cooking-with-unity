using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order
{
    //the type of potion on the order
    public PotionType OrderType;

    //the numer of potions ordered
    public int Number;

    public Order(PotionType Type, int NumPotions) {
        this.OrderType = Type;
        this.Number = NumPotions;
    }
}
