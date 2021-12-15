using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectFind;

public class ingredient : MonoBehaviour
{
    private GameObject potController;

    public Name name;

    private float buoyancy = 12.0f;

    // Start is called before the first frame update
    void Start()
    {
        //initialize water controller
        potController = FindFirstWithTag("GameController");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float upwardsForce = this.GetComponent<Rigidbody>().mass * buoyancy;
         if (isUnderWater()){
            //push the ingredient up to water height
            this.GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, upwardsForce, 0.0f), ForceMode.Force);
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
