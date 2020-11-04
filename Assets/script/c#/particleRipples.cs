using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleRipples : MonoBehaviour
{

    public GameObject water;

    private GameObject attachedTo;



    private int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //move this particle effect to its attached component
        //want to keep it at the water height
        if (attachedTo != null){
            float height = water.GetComponent<potwater>().getHeightAtPosition(this.transform.position);
            Vector3 position = attachedTo.transform.position;
            position.y = water.transform.position.y - water.GetComponent<potwater>().maxHeight/2;
            this.transform.position = position;
            count++;
        }
        setShaderProperties();
    }


    private void setShaderProperties(){
        this.GetComponent<Renderer>().material.SetTexture("_HeightMap", water.GetComponent<potwater>().heightMap);
        this.GetComponent<Renderer>().material.SetFloat("_WaterLevel", water.GetComponent<Transform>().position.y);
        this.GetComponent<Renderer>().material.SetFloat("_WaterSize", water.GetComponent<potwater>().getSize());
        this.GetComponent<Renderer>().material.SetFloat("_MaxHeight", water.GetComponent<potwater>().maxHeight);
        this.GetComponent<Renderer>().material.SetVector("_Position", this.transform.position);
        this.GetComponent<Renderer>().material.SetVector("_PotCenter", water.GetComponent<potwater>().GetCenter());
        this.GetComponent<Renderer>().material.SetInt("_Counter", count);

        //if not attached to anything, dont set these shader properties
        if (attachedTo != null){
            this.GetComponent<Renderer>().material.SetVector("_ItemWorldPosition", attachedTo.transform.position);  
            this.GetComponent<Renderer>().material.SetFloat("_Magnitude", attachedTo.GetComponent<underwater>().RippleMagnitude);
            this.GetComponent<ParticleSystem>().startSize = attachedTo.GetComponent<underwater>().RippleMagnitude;
        }
        
    }

    public void SetAttachedObject(GameObject toAttach){
        attachedTo = toAttach;
    }
}
