using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ingredient : MonoBehaviour
{
 
    public GameObject waterController;

    private Vector3 force = new Vector3(0.0f,10.0f,0.0f);

    public GameObject[] DissolvedParts;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        force.y = this.GetComponent<Rigidbody>().mass * 9.81f;
        float waterHeight = waterController.GetComponent<potController>().GetWaterHeightAtPosition(this.transform.position);
        if (this.transform.position.y < (waterHeight + waterController.GetComponent<potController>().GetWaterPosition().y)){
            this.GetComponent<Rigidbody>().AddForce(force, ForceMode.Force);
        }
    }

    public enum Name
    {
        Mushroom,
        Cinamon,
        Flower
    }

}
