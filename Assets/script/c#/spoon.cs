using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spoon : MonoBehaviour
{
    //get the origin of the spoon so we can reset its position
    Vector3 origin;
    Quaternion rotation;

    //reference the water we are currently stirring
    public GameObject water;

    //the multiplier for the force this spoon exhubits on water
    public float forceMultiplier = 2.0f;

    public float maxForce = 0.7f;

    [Range(0.0f, 1000.0f)]
    public float stirForce = 1.0f;

    private Vector3 previousPosition;

    private Vector3 position;

    private int count;
    // Start is called before the first frame update
    void Start()
    {
        origin = this.transform.position;
        previousPosition = origin;
        rotation = this.transform.rotation;
    }

    private void addForceToWater(){
        Vector3 center = water.GetComponent<potController>().GetCenter();
        float force = (this.transform.position - previousPosition).magnitude;
        Vector3 p1 = this.transform.position;
        Vector3 p2 = previousPosition;
        Vector3 ac = center - p1;
        Vector3 bc = p2 - center;
        float x = -1.0f * (ac.x*bc.z-ac.z*bc.x);
        //if x positive, then clockwise, otherwise anti clockwise
        water.GetComponent<potController>().AddForceToWater(this.transform.position, force * forceMultiplier, x * stirForce);
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 dir =  this.transform.position - water.GetComponent<potController>().GetCenter();
        dir.y = 0.0f;

        //add a force to water each frame
        this.addForceToWater();
        //detect mouse click to move spoon
        if (Input.GetMouseButton(1) && !Input.GetKey("z")){
            Vector3 mousePos = Input.mousePosition;
            position.x = (mousePos.x-500)/500;
            position.y = this.transform.position.y;
            position.z = mousePos.y/500;

            transform.localPosition = position;
            transform.right = dir;
        }else{
            //when there is no input, bring the spoon back the origin
            transform.position = origin;
            transform.rotation = rotation;
        }
        //only sample the position every 2 frames
        //if we dont do this, we wont get a big enough difference is spacing to sample a distance
        if (count % 2 == 0) {
            previousPosition = this.transform.position;
        }
        count++;
    }

}
