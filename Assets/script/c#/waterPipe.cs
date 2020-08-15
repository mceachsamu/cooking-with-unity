using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterPipe : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        shapes3D shapeGen = new shapes3D();
        Mesh mesh = shapeGen.CreateCylandar(0.1f,1,50,10);
        this.GetComponent<MeshFilter>().mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
