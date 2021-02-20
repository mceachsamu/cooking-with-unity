using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class volumeController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        setShaderProperties();
    }
    private void setShaderProperties(){
    this.GetComponent<Renderer>().material.SetVector("_Center", this.transform.position);
    }
}
