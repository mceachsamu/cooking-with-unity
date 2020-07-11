using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spoon : MonoBehaviour
{

    Vector3 origin;
    Quaternion rotation;

    public GameObject lid;

    //the dimensions of the lid
    Vector3 center;

    public GameObject Light;
    float xRad;
    float zRad;
    // Start is called before the first frame update
    void Start()
    {
        origin = this.transform.position;
        rotation = this.transform.rotation;
       // center =  lid.GetComponent<lid>().transform.position;
       // xRad = lid.GetComponent<lid>().lidXradius;
       // zRad = lid.GetComponent<lid>().lidZradius;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1)){
        Vector3 mousePos = Input.mousePosition;
        Vector3 flatPosition = new Vector3(mousePos.x/1000, this.transform.position.y, mousePos.y/1000);
        transform.localPosition = flatPosition;
        transform.rotation = new Quaternion(0.69f, 0.002f, 0.0f,0.72f);
        }else{
            transform.position = origin;
            transform.rotation = rotation;
        }
        this.GetComponent<Renderer>().material.SetVector("_LightPos", Light.transform.position);
    }
}
