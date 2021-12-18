using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectFind;

public class Underwater : MonoBehaviour
{
    private GameObject potController;

    private ParticleSystem prefab;

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
        this.GetComponent<Renderer>().material.SetFloat("_WaterOpaqueness", potController.GetComponent<potController>().GetWaterOpaqueness());
        this.GetComponent<Renderer>().material.SetFloat("_WaterSize", potController.GetComponent<potController>().GetWaterSize());
        this.GetComponent<Renderer>().material.SetTexture("_HeightMap", potController.GetComponent<potController>().GetWaterHeightMap());
        this.GetComponent<Renderer>().material.SetVector("_PotCenter", potController.GetComponent<potController>().GetCenter());
        this.GetComponent<Renderer>().material.SetFloat("_WaterLevel", potController.GetComponent<potController>().GetWaterPosition().y);
        this.GetComponent<Renderer>().material.SetFloat("_MaxHeight", potController.GetComponent<potController>().GetWaterMaxHeight());
        this.GetComponent<Renderer>().material.SetInt("_CullAboveWater", cullAboveWater);

        this.GetComponent<Renderer>().material.SetFloat("_Angle", potController.GetComponent<potController>().GetWaterAngle());
    }

}
