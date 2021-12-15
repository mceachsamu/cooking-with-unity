using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraPosition : MonoBehaviour
{
    public string KeyStroke;

    public PositionType position;

    public enum PositionType
    {
        POTION,
        KITCHEN,
        WINDOW,
    }
}
