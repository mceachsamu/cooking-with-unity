using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectFind
{
    public static GameObject FindFirstWithTag(string tag){
        GameObject[] objects = new GameObject[0];
        objects = GameObject.FindGameObjectsWithTag(tag);

        if (objects.Length > 0){
            //just get the first one
            return objects[0];
        }
        return null;
    }
}