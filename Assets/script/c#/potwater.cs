﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
public class potwater : MonoBehaviour
{
    //the bubblePrefab for the bubbles
    public GameObject bubblePrefab;
    //the pot that the water is sitting in
    public GameObject pot;

    //the object who's transformation defines where the water can be seen through
    public GameObject lid;

    //the number of segments the plane is made up from
    public int numSegs = 10;
    //the size of the segments on the plane generated
    public float segSize = 5.0f;

    //how quickly objects fade in water
    [Range(0.0f, 3.0f)]
    public float waterOpaqueness = 1.0f;

    //a counter to be used with bubbles
    float count = 0.0f;

    //the speed the bubbles pop
    [Range(0.0f, 0.5f)]
    public float speed = 0.1f;
    //the number of bubbles that appear on the water
    public int numBubbles = 20;
    //the list of bubble game objects
    private GameObject[] bubbles;

    //the main color of the water
    public Color primaryCol = new Color(155.0f/255,0.0f/255.0f,28.0f/255.0f);
    //the shading color of the water
    public Color secondaryCol = new Color(247.0f/255.0f,111.0f/255.0f,135.0f/255.0f);
    //the number of points defining the phsyical water mesh (complexity of (numFieldPoints ^ 2) * 4)
    public int numFieldPoints = 10;
    //the physical water mesh points, these define the y positions of the water surface
    private point[,] pointField;

    //the resulting texture from the points field
    public Texture2D heightMap;

    //list our constants for our water physics

    //neighbourFriction defines how much neighbouring points influence each other
    [Range(0.0f, 0.3f)]
    public float neighbourFriction = 0.1f;
    //friction defines amount of counter force for each point (higher friction const -> less friction)
    [Range(0.8f, 1.0f)]
    public float friction = 0.9f;
    //drag defines how quickly the acceleration of a point is decreased at each frame (higher drag const -> less drag)
    [Range(0.9f, 1.0f)]
    public float drag = 0.97f;
    //maxHeight defines the height of the water that we allow. at lower values the water surface is more defined. higher less defined but smoother
    [Range(0.1f, 5.0f)]
    public float maxHeight = 1.0f;
    //the mass defines the mass of each point. this effects how much points are effected by forces
    [Range(1.0f, 10.0f)]
    public float mass = 3.0f;
    //deceleration defines the rate at which each point tends to a state of rest
    [Range(-0.01f, -0.1f)]
    public float deceleration = -0.1f;

    [Range(0.01f, 0.3f)]
    public float damping = 0.1f;

    private float angle = 0.0f;

    private float angleDiff = 0.0f;

    [Range(0.6f, 1.0f)]

    public float rotationalFriction = 0.8f;

    private Quaternion startRotation;

    private float totalVolume = 0.001f;

    void Start()
    {
        Application.targetFrameRate = 60;
        //set the same as xradius for now
        //create a new 2d plane mesh for this object
        shapes3D shapeGen = new shapes3D();
        Mesh mesh = shapeGen.CreatePlane(numSegs, segSize);
        this.GetComponent<MeshFilter>().mesh = mesh;
        //initialize our field points
        this.pointField = initializePoints(numFieldPoints);

        //initilize our bubbles
        bubbles = new GameObject[numBubbles];
        for (int i = 0; i < numBubbles;i++){
            bubbles[i] = createBubble(i);
        };
        //initialize our height map texture to be the same dimensions as our field points
        heightMap = new Texture2D(numFieldPoints, numFieldPoints);

        startRotation = this.transform.rotation;

    }


    // Update is called once per frame
    void Update()
    {

        secondaryCol = primaryCol;
        //count is an internal timer we give to the shader to move the water along with time
        count = count + speed;
        //this loop applies the physics model for each point in our field an updates the heightmap accordingly
        for (int i = 0; i < numFieldPoints;i++){
            for (int j = 0; j < numFieldPoints;j++){
                pointField[i,j].move();
                heightMap.SetPixel(i,j, pointField[i,j].GetHeightValue());
            }
        }
        heightMap.Apply();

        setShaderProperties();

        //rotate the water
        angle += angleDiff;
        angle *= rotationalFriction;
        this.transform.rotation = startRotation;
        Quaternion q = RotateAroundQ(this.transform, Vector3.up, angle);

        //find out what the hieght should be given the volume and move the water to that height
        applyWaterHeight();
    }

    private void setShaderProperties(){
        //radius of the shape will be the mesh size * the scaling of the object
        float xRad = pot.transform.localScale.x * lid.GetComponent<lid>().lidXradius;
        float zRad = pot.transform.localScale.z * lid.GetComponent<lid>().lidZradius;

        //give these values to our shader
        this.GetComponent<Renderer>().material.SetFloat("xRad", xRad);
        this.GetComponent<Renderer>().material.SetFloat("zRad", zRad);
        this.GetComponent<Renderer>().material.SetFloat("seperation", segSize);
        this.GetComponent<Renderer>().material.SetFloat("totalSize", getSize());
        this.GetComponent<Renderer>().material.SetFloat("_MaxHeight", maxHeight);

        this.GetComponent<Renderer>().material.SetVector("center", GetCenter());
        this.GetComponent<Renderer>().material.SetTexture("_Tex", heightMap);
        this.GetComponent<Renderer>().material.SetVector("baseColor", primaryCol);
        this.GetComponent<Renderer>().material.SetVector("secondaryColor", secondaryCol);
    }

    //calculates what the height of the water should be based on water volume
    private void applyWaterHeight(){
        Vector3 pos = this.transform.position;

        float newHeight = Mathf.Log(totalVolume+0.5f, 2);
        pos.y = newHeight;
        this.transform.position = pos;
    }

    public void AddLiquidToWater(float amount, Color color) {
        totalVolume += amount;
    }

    public void AddForceToWater(Vector3 position, float forceAmount, float rotationAdd){
        //first rotate the input position to match the rotation of the water
        Vector3 adjustedPosition = RotateAround(position, this.transform.position, Vector3.up, -angle);
        //ensures we dont get an out of bounds exception and translates position to water
        Vector2 index = getClosestPoint(adjustedPosition);
        pointField[(int)index.x,(int)index.y].addForce(-1 * forceAmount);
        //add rotation to the water
        angleDiff += rotationAdd;
    }


    public float getHeightAtPosition(Vector3 position){
        Vector3 adjustedPosition = RotateAround(position, this.transform.position, Vector3.up, -angle);
        Vector2 closest = getClosestPoint(adjustedPosition);
        return (this.heightMap.GetPixel((int)closest.x, (int)closest.y).r - maxHeight);
    }

    //get the center point for the pot
    public Vector3 GetCenter(){
        Vector3 center = lid.transform.position;
        return center;
    }

    public Vector3 RotateAround(Vector3 position, Vector3 point, Vector3 axis, float angle)
    {
        Vector3 pos = position;
        Quaternion rotation = Quaternion.AngleAxis(angle * 0.0174532924f, axis);
        Vector3 dir = pos - point;
        dir = rotation * dir;
        pos = point + dir;
        return pos;
    }

    public Quaternion RotateAroundQ(Transform t, Vector3 axis, float angle)
    {
        Quaternion q = Quaternion.AngleAxis(angle * 0.0174532924f, axis);
        return t.rotation *= q;
    }


    public float getSize(){
        return segSize * numSegs;
    }

    public float getCount(){
        return count;
    }

    public float getXRadius(){
        return pot.transform.localScale.x * lid.GetComponent<lid>().lidXradius;
    }

    public float getZRadius(){
        return pot.transform.localScale.z * lid.GetComponent<lid>().lidZradius;
    }

    public float getSpeed(){
        return speed;
    }


    //translates wolrd positions into the closest index on the field points matrix.
    public Vector2 getClosestPoint(Vector3 position){
        float xDiff = position.x - this.transform.position.x + this.getSize()/2.0f;
        float zDiff = position.z - this.transform.position.z + this.getSize()/2.0f;
        float x = (xDiff / (this.getSize())) * numFieldPoints;
        float z = (zDiff / (this.getSize())) * numFieldPoints;

        if (x >= numFieldPoints){
            x = numFieldPoints - 1;
        }
        if (x < 0.0f){
            x = 0.0f;
        }
        if (z >= numFieldPoints){
            z = numFieldPoints - 1;
        }
        if (z < 0.0f){
            z = 0.0f;
        }

        return new Vector2(x, z);
    }

    //initializes field points
    private point[,] initializePoints(int numPoints) {
        //initialize all the points without neighbours
        int counter = 0;
        point[,] points = new point[numPoints,numPoints];

        for (int i = 0; i < numPoints;i++){
            for (int j = 0; j < numPoints; j++){
                counter++;
                points[i,j] = new point(this, 0.0f);
            }
        }
        //now that we have initialized all our points, lets set all their neighbours
        //we have each point "connected" to adjascent points so that we can cascade physic effects
        //over the matrix
        for (int i = 0; i < numPoints;i++){
            for (int j = 0; j < numPoints; j++){
                List<point> neighbours = new List<point>();
                if (i + 1 < numPoints){
                    neighbours.Add(points[i+1,j]);
                    if (j + 1 < numPoints){
                        neighbours.Add(points[i+1,j+1]);
                    }
                    if (j - 1 >= 0){
                        neighbours.Add(points[i+1,j-1]);
                    }
                }
                if (i - 1 >= 0){
                    neighbours.Add(points[i-1,j]);
                    if (j + 1 < numPoints){
                        neighbours.Add(points[i-1,j+1]);
                    }
                    if (j - 1 >= 0){
                        neighbours.Add(points[i-1,j-1]);
                    }
                }
                if (j + 1 < numPoints){
                    neighbours.Add(points[i,j+1]);
                }
                if (j - 1 >= 0){
                    neighbours.Add(points[i,j-1]);
                }
                points[i,j].setNeighbours(neighbours.ToArray());
            }
        }
        return points;
    }
    //initilizes bubble object
    private GameObject createBubble(int i){
        GameObject bub = new GameObject("Bubble-" + i);

        bub.AddComponent<MeshFilter>();
        bub.AddComponent<MeshRenderer>();
        bub.GetComponent<MeshRenderer>().material = bubblePrefab.GetComponent<MeshRenderer>().material;
        bub.GetComponent<MeshFilter>().mesh = bubblePrefab.GetComponent<MeshFilter>().mesh;
        bub.AddComponent<bubble>();
        bub.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
        bub.GetComponent<bubble>().center = new Vector3(pot.transform.position.x, lid.transform.position.y, pot.transform.position.z);
        bub.GetComponent<Renderer>().material.SetVector("baseColor", secondaryCol);
        bub.GetComponent<Renderer>().material.SetVector("secondaryColor", primaryCol);

        //render after water
        bub.GetComponent<Renderer>().material.renderQueue = 3000;
        bub.GetComponent<bubble>().scaleIncrease = Random.Range(0.003f,0.006f);
        bub.GetComponent<bubble>().maxScale = Random.Range(0.05f,0.35f);
        bub.GetComponent<bubble>().water = this;
        bub.GetComponent<Transform>().parent = this.GetComponent<Transform>();
        //water layer
        bub.layer = 4;

        return bub;
    }

}