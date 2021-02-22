using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectFind;

public class ingredient : MonoBehaviour
{

    private GameObject potController;

    private Vector3 force = new Vector3(0.0f,10.0f,0.0f);

    public GameObject[] DissolvedParts;

    // Start is called before the first frame update
    void Start()
    {
        //initialize water controller
        potController = FindFirstWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        force.y = this.GetComponent<Rigidbody>().mass * 9.81f;
        float waterHeight = potController.GetComponent<potController>().GetWaterHeightAtPosition(this.transform.position);
        if (this.transform.position.y < (waterHeight + potController.GetComponent<potController>().GetWaterPosition().y)){
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
