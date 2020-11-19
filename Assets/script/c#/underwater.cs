using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class underwater : MonoBehaviour
{
    public GameObject water;

    public ParticleSystem prefab;
    public bool hasRipples = true;
    //if true, will not render fragments above water surface
    public int cullAboveWater = 1;

    [Range(0.0f, 5.0f)]
    public float RippleMagnitude = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        //set the render queue for underwater objects
        this.GetComponent<Renderer>().material.renderQueue = 2;
        
        //create ripple effect if we hasRipples is true
        if (hasRipples){
            ParticleSystem ripple = Instantiate(prefab, this.transform.position, this.transform.rotation);
            ripple.GetComponent<particleRipples>().SetAttachedObject(this.gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        setShaderProperties();
    }

    private void setShaderProperties(){
        this.GetComponent<Renderer>().material.SetFloat("_WaterOpaqueness", water.GetComponent<potController>().GetWaterOpaqueness());
        this.GetComponent<Renderer>().material.SetFloat("_WaterSize", water.GetComponent<potController>().GetWaterSize());
        this.GetComponent<Renderer>().material.SetTexture("_HeightMap", water.GetComponent<potController>().GetWaterHeightMap());
        this.GetComponent<Renderer>().material.SetVector("_PotCenter", water.GetComponent<potController>().GetCenter());
        this.GetComponent<Renderer>().material.SetFloat("_WaterLevel", water.GetComponent<potController>().GetWaterPosition().y);
        this.GetComponent<Renderer>().material.SetFloat("_MaxHeight", water.GetComponent<potController>().GetWaterMaxHeight());
        this.GetComponent<Renderer>().material.SetInt("_CullAboveWater", cullAboveWater);
    }

}
