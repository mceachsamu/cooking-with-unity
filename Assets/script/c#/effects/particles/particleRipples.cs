using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectFind;

public class ParticleRipples : MonoBehaviour
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
            float height = potController.GetComponent<PotController>().GetWaterHeightAtPosition(this.transform.position);
            Vector3 position = attachedTo.transform.position;
            position.y = potController.GetComponent<PotController>().GetWaterPosition().y - potController.GetComponent<PotController>().GetWaterMaxHeight()/2;
            this.transform.position = position;
            count++;
        }
        setShaderProperties();
    }


    private void setShaderProperties(){
        this.GetComponent<Renderer>().material.SetTexture("_HeightMap", potController.GetComponent<PotController>().GetWaterHeightMap());
        this.GetComponent<Renderer>().material.SetFloat("_WaterLevel", potController.GetComponent<PotController>().GetWaterPosition().y);
        this.GetComponent<Renderer>().material.SetFloat("_WaterSize", potController.GetComponent<PotController>().GetWaterSize());
        this.GetComponent<Renderer>().material.SetFloat("_MaxHeight", potController.GetComponent<PotController>().GetWaterMaxHeight());
        this.GetComponent<Renderer>().material.SetVector("_Position", this.transform.position);
        this.GetComponent<Renderer>().material.SetVector("_PotCenter", potController.GetComponent<PotController>().GetCenter());
        this.GetComponent<Renderer>().material.SetInt("_Counter", count);
        this.GetComponent<Renderer>().material.SetFloat("_XRad", potController.GetComponent<PotController>().GetXRadius());

        //if not attached to anything, dont set these shader properties
        if (attachedTo != null){
            this.GetComponent<Renderer>().material.SetVector("_ItemWorldPosition", attachedTo.transform.position);
            this.GetComponent<Renderer>().material.SetFloat("_Magnitude", attachedTo.GetComponent<Underwater>().RippleMagnitude);
            this.GetComponent<ParticleSystem>().startSize = attachedTo.GetComponent<Underwater>().RippleMagnitude;
        }
    }

    public void SetAttachedObject(GameObject toAttach){
        attachedTo = toAttach;
    }
}
