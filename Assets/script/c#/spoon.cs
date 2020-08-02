using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spoon : MonoBehaviour
{

    Vector3 origin;
    Quaternion rotation;

    public GameObject lid;

    //reference the water we are currently stirring
    public GameObject water;

    //the dimensions of the lid
    Vector3 center;

    public GameObject Light;
    float xRad;
    float zRad;
    //the multiplier for the force this spoon exhubits on water
    public float forceMultiplier = 2.0f;

    public float maxForce = 0.7f;

    private int count = 0;

    private Vector3 previousPosition;
    // Start is called before the first frame update
    void Start()
    {
        origin = this.transform.position;
        previousPosition = origin;
        rotation = this.transform.rotation;
    }

    private void addForceToWater(){
        float force = (this.transform.position - previousPosition).magnitude;
        if (force > maxForce){
            force = maxForce;
        }
        water.GetComponent<potwater>().AddForceToWater(this.transform, force * forceMultiplier);
    }

    // Update is called once per frame
    void Update()
    {
        //add a force to water each frame
        this.addForceToWater();
        
        //detect mouse click to move spoon
        if (Input.GetMouseButton(1)){
            Vector3 mousePos = Input.mousePosition;
            Vector3 flatPosition = new Vector3((mousePos.x-500)/500, this.transform.position.y, mousePos.y/500);
            transform.localPosition = flatPosition;
            transform.rotation = new Quaternion(0.69f, 0.002f, 0.0f,0.72f);
        }else{
            //when there is no input, bring the spoon back the origin
            transform.position = origin;
            transform.rotation = rotation;
        }
        setShaderProperties();
        //only sample the position every 2 frames
        //if we dont do this, we wont get a big enough difference is spacing to sample a distance
        if (count % 2 == 0) {
            previousPosition = this.transform.position;
        }
        count++;
    }

    private void setShaderProperties(){
        this.GetComponent<Renderer>().material.SetVector("_LightPos", Light.transform.position);
        this.GetComponent<Renderer>().material.SetFloat("_WaterOpaqueness", water.GetComponent<potwater>().waterOpaqueness);
        this.GetComponent<Renderer>().material.SetFloat("_WaterSize", water.GetComponent<potwater>().getSize());
        this.GetComponent<Renderer>().material.SetTexture("_HeightMap", water.GetComponent<potwater>().heightMap);
        this.GetComponent<Renderer>().material.SetVector("_PotCenter", water.GetComponent<potwater>().getCenter());
        this.GetComponent<Renderer>().material.SetFloat("_WaterLevel", water.GetComponent<Transform>().position.y);
        this.GetComponent<Renderer>().material.SetFloat("_WaterLevel", water.GetComponent<potwater>().maxHeight);
    }

}
