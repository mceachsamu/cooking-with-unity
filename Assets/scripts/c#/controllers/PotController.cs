using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotController : MonoBehaviour
{
    private float _totalVolume = 0.001f;

    //the speed the bubbles pop
    [Range(0.0f, 0.5f)]
    public float Speed = 0.1f;

    //the number of bubbles that appear on the water
    public int NumBubbles = 20;

    public float StartHeight;

    public float MaxHeight;

    //the list of bubble game objects
    private GameObject[] _bubbles;

    public GameObject Water;

    public GameObject Lid;

    public GameObject BubblePrefab;

    // Start is called before the first frame update
    void Start()
    {
        //set the framrate
        Application.targetFrameRate = 60;

        //initilize our bubbles
        _bubbles = new GameObject[NumBubbles];
        for (int i = 0; i < NumBubbles;i++){
            _bubbles[i] = createBubble(i);
        }

        this.Water.GetComponent<Potwater>().SetLidObject(Lid);
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
        return Water.GetComponent<Potwater>().GetCenter();
    }

    public float GetWaterHeightAtPosition(Vector3 position){
        return Water.GetComponent<Potwater>().GetWaterHeightAtPosition(position);
    }

    public Vector3 GetWaterPosition(){
        return Water.transform.position;
    }

    public Texture2D GetWaterHeightMap(){
        return Water.GetComponent<Potwater>().GetHeightMap();
    }

    //calculates what the height of the water should be based on water volume
    private void ApplyWaterHeight(){
        Vector3 pos = Water.transform.position;

        //using log because it produces a graph which is kinda similar to the shape of a bowl
        float newHeight = Mathf.Log(_totalVolume, 2);
        
        if (pos.y < MaxHeight) {
            pos.y = newHeight + StartHeight;
        }
        
        Water.transform.position = pos;
    }

    private void SetWaterRadius(){
        this.Water.GetComponent<Potwater>().SetRadius(this.transform.localScale.x * Lid.GetComponent<Lid>().lidXradius);
    }

    public void AddLiquidToWater(float amount, Color color) {
        _totalVolume += amount;
    }

    public void AddForceToWater(Vector3 position, float forceAmount, float rotationAdd){
        Water.GetComponent<Potwater>().AddForceToWater(position, forceAmount, rotationAdd);
    }

    private GameObject createBubble(int i){
        GameObject bub = new GameObject("Bubble-" + i);

        bub.AddComponent<MeshFilter>();
        bub.AddComponent<MeshRenderer>();
        bub.GetComponent<MeshRenderer>().material = BubblePrefab.GetComponent<MeshRenderer>().material;
        bub.GetComponent<MeshFilter>().mesh = BubblePrefab.GetComponent<MeshFilter>().mesh;
        bub.AddComponent<Bubble>();
        bub.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
        bub.GetComponent<Bubble>().Center = new Vector3(this.transform.position.x, Lid.transform.position.y, this.transform.position.z);
        bub.GetComponent<Renderer>().material.SetVector("baseColor", Water.GetComponent<Potwater>().GetColor());

        bub.GetComponent<Bubble>().ScaleIncrease = Random.Range(0.003f,0.006f);
        bub.GetComponent<Bubble>().MaxScale = Random.Range(0.05f,0.1f);
        bub.GetComponent<Bubble>().Water = this;
        bub.GetComponent<Transform>().parent = Water.GetComponent<Transform>();

        //water layer
        bub.layer = 4;
        return bub;
    }

    public float GetWaterOpaqueness(){
        return Water.GetComponent<Potwater>().WaterOpaqueness;
    }

    public float GetWaterMaxHeight(){
        return Water.GetComponent<Potwater>().MaxHeight;
    }

    public float GetWaterAngle(){
        return Water.GetComponent<Potwater>().GetAngle();
    }

    public Color GetColor(){
        return Water.GetComponent<Potwater>().GetColor();
    }

    public float GetWaterSize(){
        return Water.GetComponent<Potwater>().SegSize * Water.GetComponent<Potwater>().NumSegs;
    }

    public float GetSpeed(){
        return Speed;
    }

    public float GetXRadius(){
        return this.transform.localScale.x * Lid.GetComponent<Lid>().lidXradius;
    }

}

