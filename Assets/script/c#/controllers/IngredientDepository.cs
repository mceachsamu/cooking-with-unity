using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectFind;

public class IngredientDepository : MonoBehaviour
{

    private GameObject water;
    private GameObject potController;

    // these are the forces that the ingredients spawn in with
    public float popSideForce = 10.0f;
    public float popUpwardsForce = 20.0f;

    public int numMushrooms = 0;
    public GameObject mushroomPrefab;
    private IngredientSettings MushroomSettings;

    public int numCinamons = 0;
    public GameObject cinamonPrefab;
    private IngredientSettings CinamonSettings;

    public int numFlowers = 0;
    public GameObject flowerPrefab;
    private IngredientSettings FlowerSettings;

    private IngredientSettings[] settings = new IngredientSettings[3];

    // Start is called before the first frame update
    void Start()
    {
        // initialize water
        water = FindFirstWithTag("Water");

        // initialize water controller
        potController = FindFirstWithTag("GameController");

        settings = new IngredientSettings[3]{
            new IngredientSettings(mushroomPrefab, numMushrooms, "q"),
            new IngredientSettings(cinamonPrefab, numCinamons, "w"),
            new IngredientSettings(flowerPrefab, numFlowers, "e")
        };


        for (int i = 0; i < settings.Length; i++){
            for (int j = 0; j < settings[i].numSpawn; j++){
                GameObject g = InstantiateIngredient(settings[i].prefab);

                // just moving the ingredients into the middle of nowhere
                Vector3 pos = g.transform.position;
                pos.x += 100.0f;
                g.transform.position = pos;

                settings[i].ingredients[j] = g;
            }
        }

        //destory the original prefabs
        Destroy(mushroomPrefab);
        Destroy(cinamonPrefab);
        Destroy(flowerPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        // for each ingredient type, check if they keystroke was hit to spawn the ingredient
        for (int i = 0; i < settings.Length; i++){
            if (Input.GetKeyDown(settings[i].keyStroke) && settings[i].index < settings[i].numSpawn){
                GameObject g = settings[i].ingredients[settings[i].index];
                AddForceToIngredient(g);

                g.transform.position = this.transform.position;
                settings[i].index++;
            }
        }
    }

    public void AddForceToIngredient(GameObject go){
        Vector3 dir = (potController.GetComponent<potController>().GetCenter() - this.transform.position) * popSideForce;
        dir += Vector3.up * popUpwardsForce;
        go.GetComponent<Rigidbody>().AddForce(dir);
    }

    public GameObject InstantiateIngredient(GameObject prefab){
        GameObject go = Instantiate(prefab);
        go.transform.parent = water.transform;
        return go;
    }
}
