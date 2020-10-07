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
    [Range(0.0f, 2.0f)]
    public float spoutAmplitude = 1.0f;
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
            this.transform.Rotate(rotAmountY/2.0f,rotAmountX/2.0f, 0.0f, Space.World);
        }else{
            //when there is no input, bring the spoon back the origin
            this.transform.rotation = rotation;
           // this.transform.position = position;
        }
        setShaderProperties();
        Vector3 fallPosition = getWaterFallPosition();
        //waterSpout.GetComponent<waterPipe>().SetFallPosition(fallPosition);
        mousePrev = Input.mousePosition;
    }

    void setShaderProperties(){
        this.GetComponent<Renderer>().material.SetVector("_LightPos", light.transform.position);
        this.GetComponent<Renderer>().material.SetFloat("_WaterOpaqueness", water.GetComponent<potwater>().waterOpaqueness);
        this.GetComponent<Renderer>().material.SetFloat("_WaterSize", water.GetComponent<potwater>().getSize());
        this.GetComponent<Renderer>().material.SetTexture("_HeightMap", water.GetComponent<potwater>().heightMap);
        this.GetComponent<Renderer>().material.SetVector("_PotCenter", water.GetComponent<potwater>().GetCenter());
        this.GetComponent<Renderer>().material.SetFloat("_WaterLevel", water.GetComponent<Transform>().position.y);
    }

    //get where on the water surface the water spout should land
    private Vector3 getWaterFallPosition(){
        Vector3 position = waterSpout.transform.position;
        //set the y value to the height of the water below this initial position
        position.y = water.GetComponent<potwater>().getHeightAtPosition(position);
        Vector3 direction = rotation * Vector3.up;
        position = position + direction * spoutAmplitude;
        return position;
    }
}
