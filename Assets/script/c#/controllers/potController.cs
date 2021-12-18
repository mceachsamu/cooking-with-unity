using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotController : MonoBehaviour
{
    private float totalVolume = 0.001f;

    //the speed the bubbles pop
    [Range(0.0f, 0.5f)]
    public float speed = 0.1f;

    //the number of bubbles that appear on the water
    public int numBubbles = 20;

    public float startHeight;

    public float maxHeight;

    //the list of bubble game objects
    private GameObject[] bubbles;

    public GameObject water;

    public GameObject lid;

    public GameObject bubblePrefab;

    // Start is called before the first frame update
    void Start()
    {
        //set the framrate
        Application.targetFrameRate = 60;

        //initilize our bubbles
        bubbles = new GameObject[numBubbles];
        for (int i = 0; i < numBubbles;i++){
            bubbles[i] = createBubble(i);
        }

        this.water.GetComponent<Potwater>().SetLidObject(lid);
    }

    // Update is called once per frame
    void Update()
    {
        // Count is an internal timer we give to the shader to move the water along with time

        // Find out what the height should be given the volume and move the water to that height
        ApplyWaterHeight();

        // Update the radius in case that has been changed during playtime
        SetWaterRadius();

    }

    public Vector3 GetCenter(){
        return water.GetComponent<Potwater>().GetCenter();
    }

    public float GetWaterHeightAtPosition(Vector3 position){
        return water.GetComponent<Potwater>().GetWaterHeightAtPosition(position);
    }

    public Vector3 GetWaterPosition(){
        return water.transform.position;
    }

    public Texture2D GetWaterHeightMap(){
        return water.GetComponent<Potwater>().GetHeightMap();
    }

    //calculates what the height of the water should be based on water volume
    private void ApplyWaterHeight(){
        Vector3 pos = water.transform.position;

        //using log because it produces a graph which is kinda similar to the shape of a bowl
        float newHeight = Mathf.Log(totalVolume, 2);
        
        if (pos.y < maxHeight) {
            pos.y = newHeight + startHeight;
        }
        
        water.transform.position = pos;
    }

    private void SetWaterRadius(){
        this.water.GetComponent<Potwater>().SetRadius(this.transform.localScale.x * lid.GetComponent<Lid>().lidXradius);
    }

    public void AddLiquidToWater(float amount, Color color) {
        totalVolume += amount;
    }

    public void AddForceToWater(Vector3 position, float forceAmount, float rotationAdd){
        water.GetComponent<Potwater>().AddForceToWater(position, forceAmount, rotationAdd);
    }

    private GameObject createBubble(int i){
        GameObject bub = new GameObject("Bubble-" + i);

        bub.AddComponent<MeshFilter>();
        bub.AddComponent<MeshRenderer>();
        bub.GetComponent<MeshRenderer>().material = bubblePrefab.GetComponent<MeshRenderer>().material;
        bub.GetComponent<MeshFilter>().mesh = bubblePrefab.GetComponent<MeshFilter>().mesh;
        bub.AddComponent<Bubble>();
        bub.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
        bub.GetComponent<Bubble>().center = new Vector3(this.transform.position.x, lid.transform.position.y, this.transform.position.z);
        bub.GetComponent<Renderer>().material.SetVector("baseColor", water.GetComponent<Potwater>().GetColor());

        bub.GetComponent<Bubble>().scaleIncrease = Random.Range(0.003f,0.006f);
        bub.GetComponent<Bubble>().maxScale = Random.Range(0.05f,0.1f);
        bub.GetComponent<Bubble>().water = this;
        bub.GetComponent<Transform>().parent = water.GetComponent<Transform>();

        //water layer
        bub.layer = 4;
        return bub;
    }

    public float GetWaterOpaqueness(){
        return water.GetComponent<Potwater>().waterOpaqueness;
    }

    public float GetWaterMaxHeight(){
        return water.GetComponent<Potwater>().maxHeight;
    }

    public float GetWaterAngle(){
        return water.GetComponent<Potwater>().GetAngle();
    }

    public Color GetColor(){
        return water.GetComponent<Potwater>().GetColor();
    }

    public float GetWaterSize(){
        return water.GetComponent<Potwater>().segSize * water.GetComponent<Potwater>().numSegs;
    }

    public float GetSpeed(){
        return speed;
    }

    public float GetXRadius(){
        return this.transform.localScale.x * lid.GetComponent<Lid>().lidXradius;
    }

}

