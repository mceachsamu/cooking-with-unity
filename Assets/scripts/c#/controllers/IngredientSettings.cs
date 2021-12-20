using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSettings {
    public int NumSpawn;
    public GameObject[] Ingredients;
    public GameObject Prefab;
    public int Index = 0;
    public string KeyStroke;

    public IngredientSettings(GameObject ingredientPrefab, int numberToSpawn, string keyDown) {

        Prefab = ingredientPrefab;

        NumSpawn = numberToSpawn;

        Ingredients = new GameObject[NumSpawn];

        KeyStroke = keyDown;
    }
}