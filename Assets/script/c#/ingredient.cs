using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ingredient : MonoBehaviour
{

    public GameObject water;

    private Vector3 force = new Vector3(0.0f,10.0f,0.0f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        force.y = this.GetComponent<Rigidbody>().mass * 9.81f;
        float waterHeight = water.GetComponent<potwater>().getHeightAtPosition(this.transform.position);
        if (this.transform.position.y < (waterHeight + water.transform.position.y)){
            this.GetComponent<Rigidbody>().AddForce(force, ForceMode.Force);
        }
    }
}
