using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bubble : MonoBehaviour
{

    public float initialScale = 0.0f;

    public float scaleIncrease = 0.02f;
    public float maxScale = 0.3f;

    public potController water;

    private float curScale;
    public Vector3 center;
    public float xRadius;
    public float zRadius;

    private float decay = 1.0f;

    private float decayRate = 0.02f;
    private bool decaying = false;

    // Start is called before the first frame update
    void Start()
    {
        //this.transform.localScale = new Vector3(initialScale,initialScale,initialScale);
        curScale = initialScale;
        float xPosition = center.x + Random.Range(-xRadius, xRadius);
        float zPosition = center.z + Random.Range(-zRadius, zRadius);
        this.transform.position = new Vector3(xPosition, center.y, zPosition);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localScale = this.transform.localScale + new Vector3(scaleIncrease/30.0f,scaleIncrease/30.0f,scaleIncrease/30.0f);
        this.transform.position = new Vector3(this.transform.position.x, center.y-0.05f, this.transform.position.z);
        if (this.transform.localScale.x > maxScale){
            //start decay
            decaying = true;
        }

        if (decaying) {
            decay -= decayRate;
        }
        if (decay <= 0){
            this.transform.localScale = new Vector3(initialScale,initialScale,initialScale);
            float xPosition = center.x + Random.Range(-xRadius, xRadius);
            float zPosition = center.z + Random.Range(-zRadius, zRadius);
            this.transform.position = new Vector3(xPosition, center.y, zPosition);
            decay = 1.0f;
            decaying = false;
        }

        setShaderProperties();
    }

    void setShaderProperties(){
        float bubbleHeight = water.GetWaterHeightAtPosition(this.transform.position);
       
        this.transform.position.Set(this.transform.position.x, water.GetWaterPosition().y + bubbleHeight, this.transform.position.z);

        this.GetComponent<Renderer>().material.SetTexture("_MainTex", water.GetWaterHeightMap());
        this.GetComponent<bubble>().zRadius = water.GetXRadius();
        this.GetComponent<bubble>().xRadius = water.GetXRadius();
        this.GetComponent<bubble>().scaleIncrease = water.GetSpeed();
        this.GetComponent<bubble>().center = new Vector3(water.transform.position.x, water.GetWaterPosition().y+bubbleHeight, water.transform.position.z);
        this.GetComponent<Renderer>().material.SetFloat("xRad", water.GetXRadius());
        this.GetComponent<Renderer>().material.SetFloat("zRad", water.GetXRadius());
        this.GetComponent<Renderer>().material.SetFloat("time", water.GetCount());
        this.GetComponent<Renderer>().material.SetFloat("waterSize", water.GetComponent<potController>().GetWaterSize());
        this.GetComponent<Renderer>().material.SetVector("center", water.GetCenter());
        this.GetComponent<Renderer>().material.SetVector("_Color", water.GetColor());
        this.GetComponent<Renderer>().material.SetFloat("_DecayAmount", decay);
        this.GetComponent<Renderer>().material.SetFloat("_MaxHeight", water.GetComponent<potController>().GetWaterMaxHeight());
        this.GetComponent<Renderer>().material.SetFloat("_WaterLevel", water.GetComponent<potController>().GetWaterPosition().y);
        this.GetComponent<Renderer>().material.SetVector("_PotCenter", water.GetComponent<potController>().GetCenter());
        this.GetComponent<Renderer>().material.SetTexture("_HeightMap", water.GetComponent<potController>().GetWaterHeightMap());
        this.GetComponent<Renderer>().material.SetFloat("_WaterSize", water.GetComponent<potController>().GetWaterSize());

    }
}
