using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectFind;

public class Bottle : MonoBehaviour
{

    //the origin of the bottle so we can reset its position
    Vector3 origin;

    //the origin of the bottles rotation, so we can reset the rotation
    Quaternion rotation;

    //the mouse position in the previous frame
    Vector3 mousePrev;

    private GameObject potController;

    //the bottom position of the bottle
    public GameObject bottom;

    // Start is called before the first frame update
    void Start()
    {
        //initialize origin positions;
        origin = this.transform.position;
        rotation = this.transform.rotation;
        mousePrev = Input.mousePosition;

        //initialize water
        potController = FindFirstWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("z")){
            Vector3 mousePos = Input.mousePosition;
            float rotAmountY = mousePrev.y - mousePos.y;
            float rotAmountX = mousePrev.x - mousePos.x;
            this.transform.RotateAround(bottom.transform.position, Vector3.up, rotAmountX/2.0f);
            this.transform.RotateAround(bottom.transform.position, this.transform.right, rotAmountY/2.0f);
            //this.transform.Rotate(rotAmountY/2.0f,rotAmountX/2.0f, 0.0f, Space.World);
        }else{
            //when there is no input, bring the spoon back the origin
            this.transform.rotation = rotation;
            this.transform.position = origin;
        }

        setShaderProperties();

        mousePrev = Input.mousePosition;
    }

    void setShaderProperties(){
        this.GetComponent<Renderer>().material.SetFloat("_WaterOpaqueness", potController.GetComponent<potController>().GetWaterOpaqueness());
        this.GetComponent<Renderer>().material.SetFloat("_WaterSize", potController.GetComponent<potController>().GetWaterSize());
        this.GetComponent<Renderer>().material.SetTexture("_HeightMap", potController.GetComponent<potController>().GetWaterHeightMap());
        this.GetComponent<Renderer>().material.SetVector("_PotCenter", potController.GetComponent<potController>().GetCenter());
        this.GetComponent<Renderer>().material.SetFloat("_WaterLevel", potController.GetComponent<potController>().GetWaterPosition().y);
    }

}
