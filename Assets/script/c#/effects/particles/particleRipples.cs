using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectFind;

public class particleRipples : MonoBehaviour
{

    private GameObject potController;

    private GameObject attachedTo;

    private int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        //initialize water
        potController = FindFirstWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        //move this particle effect to its attached component
        //want to keep it at the water height
        if (attachedTo != null) {
            float height = potController.GetComponent<potController>().GetWaterHeightAtPosition(this.transform.position);
            Vector3 position = attachedTo.transform.position;
            position.y = potController.GetComponent<potController>().GetWaterPosition().y - potController.GetComponent<potController>().GetWaterMaxHeight()/2;
            this.transform.position = position;
            count++;
        }
        setShaderProperties();
    }


    private void setShaderProperties(){
        this.GetComponent<Renderer>().material.SetTexture("_HeightMap", potController.GetComponent<potController>().GetWaterHeightMap());
        this.GetComponent<Renderer>().material.SetFloat("_WaterLevel", potController.GetComponent<potController>().GetWaterPosition().y);
        this.GetComponent<Renderer>().material.SetFloat("_WaterSize", potController.GetComponent<potController>().GetWaterSize());
        this.GetComponent<Renderer>().material.SetFloat("_MaxHeight", potController.GetComponent<potController>().GetWaterMaxHeight());
        this.GetComponent<Renderer>().material.SetVector("_Position", this.transform.position);
        this.GetComponent<Renderer>().material.SetVector("_PotCenter", potController.GetComponent<potController>().GetCenter());
        this.GetComponent<Renderer>().material.SetInt("_Counter", count);
        this.GetComponent<Renderer>().material.SetFloat("_XRad", potController.GetComponent<potController>().GetXRadius());

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
