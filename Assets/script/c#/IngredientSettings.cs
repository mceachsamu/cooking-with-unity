using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSettings {
    public int numSpawn;
    public GameObject[] ingredients;
    public GameObject prefab;
    public int index = 0;
    public string keyStroke;

    public IngredientSettings(GameObject ingredientPrefab, int numberToSpawn, string keyDown) {

        prefab = ingredientPrefab;

        numSpawn = numberToSpawn;

        ingredients = new GameObject[numSpawn];

        keyStroke = keyDown;
    }
}