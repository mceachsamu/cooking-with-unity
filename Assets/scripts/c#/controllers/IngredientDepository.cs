using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static ObjectFind;

public class IngredientDepository : MonoBehaviour
{

    private GameObject _water;
    private GameObject _potController;

    // these are the forces that the ingredients spawn in with
    public float PopSideForce = 10.0f;
    public float PopUpwardsForce = 20.0f;

    public int NumMushrooms = 0;
    public GameObject MushroomPrefab;
    public string MushroomKey;
    private IngredientSettings _mushroomSettings;


    public int NumCinamons = 0;
    public GameObject CinamonPrefab;
    public string CinamonKey;
    private IngredientSettings _cinamonSettings;

    public int NumFlowers = 0;
    public GameObject FlowerPrefab;
    public string FlowerKey;
    private IngredientSettings _flowerSettings;

    private IngredientSettings[] _settings = new IngredientSettings[3];

    // Start is called before the first frame update
    void Start()
    {
        // initialize _water
        _water = FindFirstWithTag("Water");

        // initialize _water controller
        _potController = FindFirstWithTag("GameController");

        // initialize our ingredient settings
        _settings = new IngredientSettings[3] {
            new IngredientSettings(MushroomPrefab, NumMushrooms, MushroomKey),
            new IngredientSettings(CinamonPrefab, NumCinamons, CinamonKey),
            new IngredientSettings(FlowerPrefab, NumFlowers, FlowerKey)
        };

        for (int i = 0; i < _settings.Length; i++)
        {
            for (int j = 0; j < _settings[i].numSpawn; j++)
            {
                GameObject g = InstantiateIngredient(_settings[i].prefab);

                // just moving the ingredients into the middle of nowhere
                Vector3 pos = g.transform.position;
                pos.x += 100.0f;
                g.transform.position = pos;

                _settings[i].ingredients[j] = g;
            }
        }

        //destory the original prefabs
        Destroy(MushroomPrefab);
        Destroy(CinamonPrefab);
        Destroy(FlowerPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        ListenToInput();
    }

    // For each ingredient type, check if they keystroke was hit to spawn the ingredient
    private void ListenToInput()
    {
        for (int i = 0; i < _settings.Length; i++)
        {
            if (Input.GetKeyDown(_settings[i].keyStroke) && _settings[i].index < _settings[i].numSpawn)
            {
                GameObject g = _settings[i].ingredients[_settings[i].index];
                AddForceToIngredient(g);

                g.transform.position = this.transform.position;
                _settings[i].index++;
            }
        }
    }

    public void AddForceToIngredient(GameObject ingredient)
    {
        Vector3 dir = (_potController.GetComponent<PotController>().GetCenter() - this.transform.position) * PopSideForce;
        dir += Vector3.up * PopUpwardsForce;
        ingredient.GetComponent<Rigidbody>().AddForce(dir);
    }

    public GameObject InstantiateIngredient(GameObject prefab)
    {
        GameObject go = Instantiate(prefab);

        // set the parent transform to be the _water so that the ingredients spin at the same rate as the _water
        go.transform.parent = _water.transform;
        return go;
    }
}
