﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterPipe : MonoBehaviour
{
    public float baseRadius = 0.1f;
    public float baseLength = 1.0f;
    public int numSegmentsRound = 15;
    public int numSegmentsLong = 10;

    public GameObject light;
    public GameObject water;

    public GameObject start;

    private Vector3 PreviousPoint;

    private Vector3 FallPosition;

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
        water.GetComponent<potwater>().AddForceToWater(FallPosition, 0.1f);
        this.GetComponent<Renderer>().material.SetVector("_PipeStart", this.transform.position);
        this.GetComponent<Renderer>().material.SetVector("_PipeEnd", FallPosition);

        this.GetComponent<Renderer>().material.SetFloat("_Count", count);
        this.GetComponent<Renderer>().material.SetFloat("_PipeLength", baseLength);
        this.GetComponent<Renderer>().material.SetVector("_PreviousEnd", PreviousPoint);
        this.GetComponent<Renderer>().material.SetVector("_LightPos", light.transform.position);

        float diffX = FallPosition.x - PreviousPoint.x;
        float diffY = FallPosition.y - PreviousPoint.y;
        float diffZ = FallPosition.z - PreviousPoint.z;
        PreviousPoint.x += diffX * creep;
        PreviousPoint.y += diffY * creep;
        PreviousPoint.z += diffZ * creep;
    }

    public void SetFallPosition(Vector3 position) {
        FallPosition = position;
    }
}
