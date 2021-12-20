using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectFind;

public class UnderwaterBubbles : MonoBehaviour
{

    private GameObject potController;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Renderer>().material.renderQueue = 3;

        //initialize water
        potController = FindFirstWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        setShaderProperties();
    }

    private void setShaderProperties(){
        this.GetComponent<Renderer>().material.SetFloat("_WaterOpaqueness", potController.GetComponent<PotController>().GetWaterOpaqueness());
        this.GetComponent<Renderer>().material.SetFloat("_WaterSize", potController.GetComponent<PotController>().GetWaterSize());
        this.GetComponent<Renderer>().material.SetTexture("_HeightMap", potController.GetComponent<PotController>().GetWaterHeightMap());
        this.GetComponent<Renderer>().material.SetVector("_PotCenter", potController.GetComponent<PotController>().GetCenter());
        this.GetComponent<Renderer>().material.SetFloat("_WaterLevel", potController.GetComponent<PotController>().GetWaterPosition().y);
        this.GetComponent<Renderer>().material.SetFloat("_MaxHeight", potController.GetComponent<PotController>().GetWaterMaxHeight());
    }
}
