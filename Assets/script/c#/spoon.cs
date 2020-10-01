using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spoon : MonoBehaviour
{
    //get the origin of the spoon so we can reset its position
    Vector3 origin;
    Quaternion rotation;

    public GameObject lid;

    //reference the water we are currently stirring
    public GameObject water;

    //the dimensions of the lid
    Vector3 center;

    public GameObject Light;
    float xRad;
    float zRad;
    //the multiplier for the force this spoon exhubits on water
    public float forceMultiplier = 2.0f;

    public float maxForce = 0.7f;

    private int count = 0;

    private Quaternion defaultRotation = new Quaternion(0.69f, 0.002f, 0.0f,0.72f);

    private Vector3 previousPosition;

    private Vector3 position;
    // Start is called before the first frame update
    void Start()
    {
        origin = this.transform.position;
        previousPosition = origin;
        rotation = this.transform.rotation;
    }

    private void addForceToWater(){
        float force = (this.transform.position - previousPosition).magnitude;
        if (force > maxForce){
            force = maxForce;
        }
        water.GetComponent<potwater>().AddForceToWater(this.transform.position, force * forceMultiplier);
    }

    // Update is called once per frame
    void Update()
    {
        //add a force to water each frame
        this.addForceToWater();
        //detect mouse click to move spoon
        if (Input.GetMouseButton(1) && !Input.GetKey("z")){
            Vector3 mousePos = Input.mousePosition;
            position.x = (mousePos.x-500)/500;
            position.y = this.transform.position.y;
            position.z = mousePos.y/500;

            transform.localPosition = position;
            transform.rotation = defaultRotation;
        }else{
            //when there is no input, bring the spoon back the origin
            transform.position = origin;
            transform.rotation = rotation;
        }
        setShaderProperties();
        //only sample the position every 2 frames
        //if we dont do this, we wont get a big enough difference is spacing to sample a distance
        if (count % 2 == 0) {
            previousPosition = this.transform.position;
        }
        count++;
    }

    private void setShaderProperties(){
        this.GetComponent<Renderer>().material.SetVector("_LightPos", Light.transform.position);
    }

}
