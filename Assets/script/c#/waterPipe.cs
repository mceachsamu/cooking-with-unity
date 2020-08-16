using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterPipe : MonoBehaviour
{
    public float baseRadius = 0.1f;
    public float baseLength = 1.0f;
    public int numSegmentsRound = 15;
    public int numSegmentsLong = 10;
    public GameObject water;

    public GameObject start;

    private Vector3 PreviousPoint;

    private int count = 0;

    public float creep = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        shapes3D shapeGen = new shapes3D();
        Mesh mesh = shapeGen.CreateCylandar(baseRadius,baseLength,numSegmentsRound,numSegmentsLong);
        this.GetComponent<MeshFilter>().mesh = mesh;
        PreviousPoint = water.GetComponent<potwater>().getCenter();
    }

    // Update is called once per frame
    void Update()
    {
        count++;
        this.GetComponent<Renderer>().material.SetVector("_PipeStart", this.transform.position);
        this.GetComponent<Renderer>().material.SetVector("_PipeEnd", water.GetComponent<potwater>().getCenter());
        this.GetComponent<Renderer>().material.SetFloat("_Count", count);
        this.GetComponent<Renderer>().material.SetFloat("_PipeLength", baseLength);
        this.GetComponent<Renderer>().material.SetVector("_PreviousEnd", PreviousPoint);
        
        float diffX = water.GetComponent<potwater>().getCenter().x - PreviousPoint.x;
        float diffY = water.GetComponent<potwater>().getCenter().y - PreviousPoint.y;
        float diffZ = water.GetComponent<potwater>().getCenter().z - PreviousPoint.z;
        
        PreviousPoint.x += diffX * creep;
        PreviousPoint.y -= diffY * creep;
        PreviousPoint.z += diffZ * creep;
    }

    //rotates this object to face a point
    private void RotateToPoint(Vector3 point) {
        Vector3 position = this.GetComponent<Transform>().position;
        
    }
}
