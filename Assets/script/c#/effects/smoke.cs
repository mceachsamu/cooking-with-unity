using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectFind;

public class Smoke : MonoBehaviour
{
    //a counter that increments each update. used to move smoke effect with time
    private int count = 0;

    private GameObject potController;

    // Start is called before the first frame update
    void Start()
    {
        //initialize water
        potController = FindFirstWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        //move upwards with the water level
        Vector3 pos = this.transform.position;
        pos.y = potController.transform.position.y;
        this.transform.position = pos;

        count++;
        SetShaderProperties();
    }

    private void SetShaderProperties(){
        this.GetComponent<Renderer>().material.SetInt("_Count", count);
        this.GetComponent<Renderer>().material.SetVector("_Color", potController.GetComponent<PotController>().GetColor());
    }
}
