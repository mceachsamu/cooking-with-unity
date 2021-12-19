using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectFind;

public class Underwater : MonoBehaviour
{
    private GameObject potController;
    public bool hasRipples = true;

    //if true, will not render fragments above water surface
    public int cullAboveWater = 1;

    [Range(0.0f, 5.0f)]
    public float RippleMagnitude = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        //initialize water
        potController = FindFirstWithTag("GameController");

        //set the render queue for underwater objects
        this.GetComponent<Renderer>().material.renderQueue = 2;

        //create ripple effect if we hasRipples is true
        if (hasRipples){
            GameObject particles = FindFirstWithTag("WaterRipples");
            ParticleSystem system = particles.GetComponent<ParticleSystem>();
            ParticleSystem ripple = Instantiate(system, this.transform.position, this.transform.rotation);
            ripple.GetComponent<ParticleRipples>().SetAttachedObject(this.gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        setShaderProperties();
    }

    private void setShaderProperties(){
        Material mat = this.GetComponent<Renderer>().material;
        mat.SetFloat("_WaterOpaqueness", potController.GetComponent<PotController>().GetWaterOpaqueness());
        mat.SetFloat("_WaterSize", potController.GetComponent<PotController>().GetWaterSize());
        mat.SetTexture("_HeightMap", potController.GetComponent<PotController>().GetWaterHeightMap());
        mat.SetVector("_PotCenter", potController.GetComponent<PotController>().GetCenter());
        mat.SetFloat("_WaterLevel", potController.GetComponent<PotController>().GetWaterPosition().y);
        mat.SetFloat("_MaxHeight", potController.GetComponent<PotController>().GetWaterMaxHeight());
        mat.SetInt("_CullAboveWater", cullAboveWater);
        mat.SetFloat("_Angle", potController.GetComponent<PotController>().GetWaterAngle());
    }

}
