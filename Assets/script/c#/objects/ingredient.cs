using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectFind;

public class ingredient : MonoBehaviour
{
    private GameObject potController;

    private Vector3 force = new Vector3(0.0f,10.0f,0.0f);

    public Name name;

    private float gravityAcceleration = 9.81f;

    // Start is called before the first frame update
    void Start()
    {
        //initialize water controller
        potController = FindFirstWithTag("GameController");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        force.y = this.GetComponent<Rigidbody>().mass * gravityAcceleration;
         if (isUnderWater()){
            //push the ingredient up to water height
            this.GetComponent<Rigidbody>().AddForce(force, ForceMode.Force);
        }
    }

    private bool isUnderWater() {
        float waterHeight = potController.GetComponent<potController>().GetWaterHeightAtPosition(this.transform.position);
        return this.transform.position.y < (waterHeight + potController.GetComponent<potController>().GetWaterPosition().y);
    }

    public enum Name
    {
        Mushroom,
        Cinamon,
        Flower
    }
}
