using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spoutBubbles : MonoBehaviour
{

    public GameObject water;

    public GameObject spout;

    [Range(0.0f, 3.0f)]
    public float Asjust = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
        this.GetComponent<Renderer>().material.renderQueue = 3;
    }

    // Update is called once per frame
    void Update()
    {
        //get the position that the spout lands on the water surface
        Vector3 pos = spout.GetComponent<waterPipe>().GetFallPosition();
        pos.y -= Asjust;
        this.transform.position = pos;
        setShaderProperties();
    }

    private void setShaderProperties(){
        this.GetComponent<Renderer>().material.SetFloat("_WaterOpaqueness", water.GetComponent<potController>().GetWaterOpaqueness());
        this.GetComponent<Renderer>().material.SetFloat("_WaterSize", water.GetComponent<potController>().GetWaterSize());
        this.GetComponent<Renderer>().material.SetTexture("_HeightMap", water.GetComponent<potController>().GetWaterHeightMap());
        this.GetComponent<Renderer>().material.SetVector("_PotCenter", water.GetComponent<potController>().GetCenter());
        this.GetComponent<Renderer>().material.SetFloat("_WaterLevel", water.GetComponent<potController>().GetWaterPosition().y);
        this.GetComponent<Renderer>().material.SetFloat("_MaxHeight", water.GetComponent<potController>().GetWaterMaxHeight());
    }
}
