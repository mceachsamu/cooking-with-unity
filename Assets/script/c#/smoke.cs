using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smoke : MonoBehaviour
{
    //a counter that increments each update. used to move smoke effect with time
    private int count = 0;

    public GameObject water;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //move upwards with the water level
        Vector3 pos = this.transform.position;
        pos.y = water.transform.position.y;
        this.transform.position = pos;

        count++;
        SetShaderProperties();
    }

    private void SetShaderProperties(){
        this.GetComponent<Renderer>().material.SetInt("_Count", count);
        this.GetComponent<Renderer>().material.SetVector("_Color", water.GetComponent<potController>().GetColor());
    }
}
