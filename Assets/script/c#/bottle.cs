using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bottle : MonoBehaviour
{


    //get the origin of the bottle so we can reset its position
    Vector3 origin;
    Quaternion rotation;

    Vector3 mousePrev;

    public GameObject water;
    public GameObject light;
    public GameObject waterSpout;
    private Vector3 previousPosition;
    public GameObject bottom;
    // Start is called before the first frame update
    void Start()
    {
        origin = this.transform.position;
        previousPosition = origin;
        rotation = this.transform.rotation;
        mousePrev = Input.mousePosition;
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
        this.GetComponent<Renderer>().material.SetFloat("_WaterOpaqueness", water.GetComponent<potController>().GetWaterOpaqueness());
        this.GetComponent<Renderer>().material.SetFloat("_WaterSize", water.GetComponent<potController>().GetWaterSize());
        this.GetComponent<Renderer>().material.SetTexture("_HeightMap", water.GetComponent<potController>().GetWaterHeightMap());
        this.GetComponent<Renderer>().material.SetVector("_PotCenter", water.GetComponent<potController>().GetCenter());
        this.GetComponent<Renderer>().material.SetFloat("_WaterLevel", water.GetComponent<potController>().GetWaterPosition().y);
    }

}
