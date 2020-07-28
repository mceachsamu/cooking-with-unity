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
            this.transform.localScale = new Vector3(initialScale,initialScale,initialScale);
            float xPosition = center.x + Random.Range(-xRadius, xRadius);
            float zPosition = center.z + Random.Range(-zRadius, zRadius);
            this.transform.position = new Vector3(xPosition, center.y, zPosition);
        }
    }

    void setShaderProperties(){
        Vector2 bubbleIndex = water.getClosestPoint(this.transform);
            float bubbleHeight = water.heightMap.GetPixel((int)bubbleIndex.x, (int)bubbleIndex.y).r;
            this.transform.position.Set(this.transform.position.x, this.transform.position.y+bubbleHeight, this.transform.position.z);
            this.GetComponent<Renderer>().material.SetTexture("_MainTex", water.heightMap);
            // this.GetComponent<bubble>().zRadius = water.zRad;
            // this.GetComponent<bubble>().xRadius = water.xRad;
            this.GetComponent<bubble>().scaleIncrease = water.getSpeed();
//            this.GetComponent<bubble>().center = new Vector3(pot.transform.position.x, water.transform.position.y+bubbleHeight, pot.transform.position.z);
            this.GetComponent<Renderer>().material.SetFloat("xRad", water.getXRadius());
            this.GetComponent<Renderer>().material.SetFloat("zRad", water.getXRadius());
            this.GetComponent<Renderer>().material.SetFloat("time", water.getCount());
            this.GetComponent<Renderer>().material.SetFloat("waterSize", water.segSize * water.numSegs);
            this.GetComponent<Renderer>().material.SetVector("center", water.getCenter());
            this.GetComponent<Renderer>().material.SetVector("_LightPos", water.Light.transform.position);
            this.GetComponent<Renderer>().material.SetVector("_Color", water.primaryCol);
            this.GetComponent<Renderer>().material.SetVector("_Color2", water.secondaryCol);
    }
}
