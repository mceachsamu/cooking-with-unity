using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bubble : MonoBehaviour
{

    public float initialScale = 0.0f;

    public float scaleIncrease = 0.02f;
    public float maxScale = 0.3f;

    public potwater water;

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
        Vector2 bubbleIndex = water.getClosestPoint(this.transform.position);
        float bubbleHeight = water.heightMap.GetPixel((int)bubbleIndex.x, (int)bubbleIndex.y).r - water.maxHeight;
        this.transform.position.Set(this.transform.position.x, water.transform.position.y+bubbleHeight, this.transform.position.z);
        this.GetComponent<Renderer>().material.SetTexture("_MainTex", water.heightMap);
        this.GetComponent<bubble>().zRadius = water.getXRadius();
        this.GetComponent<bubble>().xRadius = water.getZRadius();
        this.GetComponent<bubble>().scaleIncrease = water.getSpeed();
        this.GetComponent<bubble>().center = new Vector3(water.pot.transform.position.x, water.transform.position.y+bubbleHeight, water.pot.transform.position.z);
        this.GetComponent<Renderer>().material.SetFloat("xRad", water.getXRadius());
        this.GetComponent<Renderer>().material.SetFloat("zRad", water.getXRadius());
        this.GetComponent<Renderer>().material.SetFloat("time", water.getCount());
        this.GetComponent<Renderer>().material.SetFloat("waterSize", water.segSize * water.numSegs);
        this.GetComponent<Renderer>().material.SetVector("center", water.getCenter());
        this.GetComponent<Renderer>().material.SetVector("_LightPos", water.Light.transform.position);
        this.GetComponent<Renderer>().material.SetVector("_Color2", water.primaryCol);
        this.GetComponent<Renderer>().material.SetVector("_Color", water.secondaryCol);
        this.GetComponent<Renderer>().material.SetFloat("_DecayAmount", decay);
        this.GetComponent<Renderer>().material.SetFloat("_MaxHeight", water.GetComponent<potwater>().maxHeight);
        this.GetComponent<Renderer>().material.SetFloat("_WaterLevel", water.GetComponent<Transform>().position.y);
        this.GetComponent<Renderer>().material.SetVector("_PotCenter", water.GetComponent<potwater>().getCenter());
        this.GetComponent<Renderer>().material.SetTexture("_HeightMap", water.GetComponent<potwater>().heightMap);
        this.GetComponent<Renderer>().material.SetFloat("_WaterSize", water.GetComponent<potwater>().getSize());

    }
}
