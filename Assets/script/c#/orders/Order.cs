using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order
{
    //the type of potion on the order
    public PotionType order;

    //the numer of potions ordered
    public int number;

    public Order(PotionType type, int numPotions) {
        this.order = type;
        this.number = numPotions;
    }
}
