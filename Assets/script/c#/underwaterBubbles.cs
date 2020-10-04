using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class underwaterBubbles : MonoBehaviour
{

    public GameObject water;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Renderer>().material.renderQueue = 3;
    }

    // Update is called once per frame
    void Update()
    {
        setShaderProperties();
    }

    private void setShaderProperties(){
        this.GetComponent<Renderer>().material.SetFloat("_WaterOpaqueness", water.GetComponent<potwater>().waterOpaqueness);
        this.GetComponent<Renderer>().material.SetFloat("_WaterSize", water.GetComponent<potwater>().getSize());
        this.GetComponent<Renderer>().material.SetTexture("_HeightMap", water.GetComponent<potwater>().heightMap);
        this.GetComponent<Renderer>().material.SetVector("_PotCenter", water.GetComponent<potwater>().GetCenter());
        this.GetComponent<Renderer>().material.SetFloat("_WaterLevel", water.GetComponent<Transform>().position.y);
        this.GetComponent<Renderer>().material.SetFloat("_MaxHeight", water.GetComponent<potwater>().maxHeight);
    }
}
