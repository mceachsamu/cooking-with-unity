using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class potController : MonoBehaviour
{
    private float totalVolume = 0.001f;
    
    //the speed the bubbles pop
    [Range(0.0f, 0.5f)]
    public float speed = 0.1f;

    private float count = 0;

    //the number of bubbles that appear on the water
    public int numBubbles = 20;

    //the list of bubble game objects
    private GameObject[] bubbles;

    public GameObject water;

    public GameObject lid;

    public GameObject pot;

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

        this.water.GetComponent<potwater>().SetLidObject(lid);
    }

    // Update is called once per frame
    void Update()
    {
        //count is an internal timer we give to the shader to move the water along with time
        count = count + speed;

        //find out what the hieght should be given the volume and move the water to that height
        ApplyWaterHeight();

        SetWaterRadius();

    }

    public Vector3 GetCenter(){
        return water.GetComponent<potwater>().GetCenter();
    }

    public float GetWaterHeightAtPosition(Vector3 position){
        return water.GetComponent<potwater>().GetWaterHeightAtPosition(position);
    }

    public Vector3 GetWaterPosition(){
        return water.transform.position;
    }

    public Texture2D GetWaterHeightMap(){
        return water.GetComponent<potwater>().GetHeightMap();
    }

    //calculates what the height of the water should be based on water volume
    private void ApplyWaterHeight(){
        Vector3 pos = water.transform.position;

        //using log because its kinda similar to the shape of a bowl
        float newHeight = Mathf.Log(totalVolume+0.5f, 2);
        pos.y = newHeight;
        water.transform.position = pos;
    }

    private void SetWaterRadius(){
        this.water.GetComponent<potwater>().SetRadius(pot.transform.localScale.x * lid.GetComponent<lid>().lidXradius);
    }

    public void AddLiquidToWater(float amount, Color color) {
        totalVolume += amount;
    }

    public void AddForceToWater(Vector3 position, float forceAmount, float rotationAdd){
        water.GetComponent<potwater>().AddForceToWater(position, forceAmount, rotationAdd);
    }

    private GameObject createBubble(int i){
        GameObject bub = new GameObject("Bubble-" + i);

        bub.AddComponent<MeshFilter>();
        bub.AddComponent<MeshRenderer>();
        bub.GetComponent<MeshRenderer>().material = bubblePrefab.GetComponent<MeshRenderer>().material;
        bub.GetComponent<MeshFilter>().mesh = bubblePrefab.GetComponent<MeshFilter>().mesh;
        bub.AddComponent<bubble>();
        bub.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
        bub.GetComponent<bubble>().center = new Vector3(pot.transform.position.x, lid.transform.position.y, pot.transform.position.z);
        bub.GetComponent<Renderer>().material.SetVector("baseColor", water.GetComponent<potwater>().GetColor());

        bub.GetComponent<bubble>().scaleIncrease = Random.Range(0.003f,0.006f);
        bub.GetComponent<bubble>().maxScale = Random.Range(0.05f,0.1f);
        bub.GetComponent<bubble>().water = this;
        bub.GetComponent<Transform>().parent = water.GetComponent<Transform>();

        //water layer
        bub.layer = 4;
        return bub;
    }

    public float GetWaterOpaqueness(){
        return water.GetComponent<potwater>().waterOpaqueness;
    }

    public float GetWaterMaxHeight(){
        return water.GetComponent<potwater>().maxHeight;
    }

    public float GetWaterAngle(){
        return water.GetComponent<potwater>().GetAngle();
    }

    public Color GetColor(){
        return water.GetComponent<potwater>().GetColor();
    }

    public float GetWaterSize(){
        return water.GetComponent<potwater>().segSize * water.GetComponent<potwater>().numSegs;
    }

    public float GetCount(){
        return count;
    }

    public float GetSpeed(){
        return speed;
    }

    public float GetXRadius(){
        return pot.transform.localScale.x * lid.GetComponent<lid>().lidXradius;
    }

}

