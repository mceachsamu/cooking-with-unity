using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pot : MonoBehaviour
{

    public GameObject Light;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Renderer>().material.SetVector("_LightPos", Light.transform.position);
        this.GetComponent<Renderer>().material.SetVector("_LightPos", Light.transform.position);

    }
}
