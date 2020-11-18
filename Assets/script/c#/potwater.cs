using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
public class potwater : MonoBehaviour
{
    //the object who's transformation defines where the water can be seen through
    private GameObject lid;

    //the number of segments the plane is made up from
    public int numSegs = 10;
    //the size of the segments on the plane generated
    public float segSize = 5.0f;

    //how quickly objects fade in water
    [Range(0.0f, 3.0f)]
    public float waterOpaqueness = 1.0f;

    //the main color of the water
    public Color primaryCol = new Color(155.0f/255,0.0f/255.0f,28.0f/255.0f);
    //the shading color of the water

    public int numFieldPoints = 10;

    //the physical water mesh points, these define the y positions of the water surface
    private point[,] pointField;

    //the resulting texture from the points field
    private Texture2D heightMap;

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

    private float xRadius;

    void Start()
    {
        //set the same as xradius for now
        //create a new 2d plane mesh for this object
        shapes3D shapeGen = new shapes3D();
        Mesh mesh = shapeGen.CreatePlane(numSegs, segSize);
        this.GetComponent<MeshFilter>().mesh = mesh;

        //initialize our field points
        this.pointField = initializePoints(numFieldPoints);

        //initialize our height map texture to be the same dimensions as our field points
        heightMap = new Texture2D(numFieldPoints, numFieldPoints);

        //store the starting rotation of this object
        startRotation = this.transform.rotation;
    }


    // Update is called once per frame
    void Update()
    {
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

    }

    public void AddForceToWater(Vector3 position, float forceAmount, float rotationAdd){
        //first rotate the input position to match the rotation of the water
        Vector3 adjustedPosition = RotateAround(position, this.transform.position, Vector3.up, -angle);
        //ensures we dont get an out of bounds exception and translates position to water
        Vector2 index = GetClosestPoint(adjustedPosition);
        pointField[(int)index.x,(int)index.y].addForce(-1 * forceAmount);
        //add rotation to the water
        angleDiff += rotationAdd;
    }

    private void setShaderProperties(){
        //give these values to our shader
        this.GetComponent<Renderer>().material.SetFloat("xRad", xRadius);
        this.GetComponent<Renderer>().material.SetFloat("seperation", segSize);
        this.GetComponent<Renderer>().material.SetFloat("totalSize", GetSize());
        this.GetComponent<Renderer>().material.SetFloat("_MaxHeight", maxHeight);

        this.GetComponent<Renderer>().material.SetVector("center", GetCenter());
        this.GetComponent<Renderer>().material.SetTexture("_Tex", heightMap);
        this.GetComponent<Renderer>().material.SetVector("baseColor", primaryCol);
    }

    public Color GetColor(){
        return primaryCol;
    }

    public void SetColor(Color col){
        primaryCol = col;
    }

    
    public float GetWaterHeightAtPosition(Vector3 position){
        Vector3 adjustedPosition = RotateAround(position, this.transform.position, Vector3.up, -angle);
        Vector2 closest = GetClosestPoint(adjustedPosition);
        return (this.heightMap.GetPixel((int)closest.x, (int)closest.y).r - maxHeight);
    }


    public float GetAngle(){
        return angle;
    }

    public void SetLidObject(GameObject lid){
        this.lid = lid;
    }

    //get the center point for the pot
    public Vector3 GetCenter(){
        Vector3 center = lid.transform.position;
        return center;
    }

    public float GetSize(){
        return segSize * numSegs;
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


    public void SetRadius(float radius){
        xRadius = radius;
    }

    public Texture2D GetHeightMap(){
        return heightMap;
    }

    //translates world positions into the closest index on the field points matrix.
    public Vector2 GetClosestPoint(Vector3 position){
        float xDiff = position.x - this.transform.position.x + this.GetSize()/2.0f;
        float zDiff = position.z - this.transform.position.z + this.GetSize()/2.0f;
        float x = (xDiff / (this.GetSize())) * numFieldPoints;
        float z = (zDiff / (this.GetSize())) * numFieldPoints;

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
    public point[,] initializePoints(int numPoints) {
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

}