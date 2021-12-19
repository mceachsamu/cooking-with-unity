using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
public class Potwater : MonoBehaviour
{
    //the object who's transformation defines where the water can be seen through
    private GameObject _lid;

    //the number of segments the plane is made up from
    public int NumSegs = 10;
    //the size of the segments on the plane generated
    public float SegSize = 5.0f;

    //how quickly objects fade in water
    [Range(0.0f, 3.0f)]
    public float WaterOpaqueness = 1.0f;

    //the main color of the water
    public Color PrimaryCol = new Color(155.0f/255,0.0f/255.0f,28.0f/255.0f);
    //the shading color of the water

    public int NumFieldPoints = 10;

    //the physical water mesh points, these define the y positions of the water surface
    private WaterPoint[,] _pointField;

    //the resulting texture from the points field
    private Texture2D _heightMap;

    //list our constants for our water physics

    //neighbourFriction defines how much neighbouring points influence each other
    [Range(0.0f, 0.3f)]
    public float NeighbourFriction = 0.1f;
    //friction defines amount of counter force for each point (higher friction const -> less friction)
    [Range(0.8f, 1.0f)]
    public float Friction = 0.9f;
    //drag defines how quickly the acceleration of a point is decreased at each frame (higher drag const -> less drag)
    [Range(0.9f, 1.0f)]
    public float Drag = 0.97f;
    //maxHeight defines the height of the water that we allow. at lower values the water surface is more defined. higher less defined but smoother
    [Range(0.1f, 5.0f)]
    public float MaxHeight = 1.0f;
    //the mass defines the mass of each point. this effects how much points are effected by forces
    [Range(1.0f, 10.0f)]
    public float Mass = 3.0f;
    //deceleration defines the rate at which each point tends to a state of rest
    [Range(-0.01f, -0.1f)]
    public float Deceleration = -0.1f;

    [Range(0.01f, 0.3f)]
    public float Damping = 0.1f;

    private float _angle = 0.0f;

    private float _angleDiff = 0.0f;

    [Range(0.6f, 1.0f)]

    public float RotationalFriction = 0.8f;

    private Quaternion _startRotation;

    private float _xRadius;

    void Start()
    {
        //set the same as xradius for now
        //create a new 2d plane mesh for this object
        Shapes3D shapeGen = new Shapes3D();
        Mesh mesh = shapeGen.CreatePlane(NumSegs, SegSize);
        this.GetComponent<MeshFilter>().mesh = mesh;

        //initialize our field points
        this._pointField = InitializePoints(NumFieldPoints);

        //initialize our height map texture to be the same dimensions as our field points
        this._heightMap = new Texture2D(NumFieldPoints, NumFieldPoints);

        //store the starting rotation of this object
        this._startRotation = this.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //this loop applies the physics model for each point in our field an updates the heightmap accordingly
        for (int i = 0; i < NumFieldPoints;i++){
            for (int j = 0; j < NumFieldPoints;j++){
                _pointField[i,j].Move();
                _heightMap.SetPixel(i,j, _pointField[i,j].GetHeightValue());
            }
        }
        _heightMap.Apply();

        setShaderProperties();

        //rotate the water
        this._angle += _angleDiff;
        this._angleDiff *= RotationalFriction;
        this.transform.rotation = _startRotation;
        Quaternion q = RotateAroundQ(this.transform, Vector3.up, _angle);
    }

    public void AddForceToWater(Vector3 position, float forceAmount, float rotationAdd){
        //first rotate the input position to match the rotation of the water
        Vector3 adjustedPosition = RotateAround(position, this.transform.position, Vector3.up, -_angle);
        
        //ensures we dont get an out of bounds exception and translates position to water
        Vector2 index = GetClosestPoint(adjustedPosition);
        this._pointField[(int)index.x,(int)index.y].AddForce(-1 * forceAmount);
        
        //add rotation to the water
        this._angleDiff += rotationAdd;
    }

    private void setShaderProperties(){
        Material mat = this.GetComponent<Renderer>().material;

        // Give these values to our shader
        mat.SetFloat("_xRad", _xRadius);
        mat.SetFloat("_Seperation", SegSize);
        mat.SetFloat("_TotalSize", GetSize());
        mat.SetFloat("_MaxHeight", MaxHeight);
        mat.SetVector("_Center", GetCenter());
        mat.SetTexture("_Tex", _heightMap);
        mat.SetVector("_BaseColor", PrimaryCol);
    }

    public Color GetColor(){
        return PrimaryCol;
    }

    public void SetColor(Color col){
        this.PrimaryCol = col;
    }


    public float GetWaterHeightAtPosition(Vector3 position){
        Vector3 adjustedPosition = RotateAround(position, this.transform.position, Vector3.up, -_angle);
        Vector2 closest = GetClosestPoint(adjustedPosition);
        return (this._heightMap.GetPixel((int)closest.x, (int)closest.y).r) - MaxHeight;
    }


    public float GetAngle(){
        return _angle;
    }

    public void SetLidObject(GameObject lid){
        this._lid = lid;
    }

    //get the center point for the pot
    public Vector3 GetCenter(){
        if (_lid != null){
            return _lid.transform.position;
        }

        return new Vector3(0.0f,0.0f,0.0f);
    }

    public float GetSize(){
        return SegSize * NumSegs;
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
        Quaternion q = Quaternion.AngleAxis(this._angle * 0.0174532924f, axis);
        return t.rotation *= q;
    }

    public void SetRadius(float radius){
        this._xRadius = radius;
    }

    public Texture2D GetHeightMap(){
        return this._heightMap;
    }

    // GetClosestPoint translates world positions into the closest index on the field points matrix.
    public Vector2 GetClosestPoint(Vector3 position){
        float xDiff = position.x - this.transform.position.x + this.GetSize()/2.0f;
        float zDiff = position.z - this.transform.position.z + this.GetSize()/2.0f;
        float x = (xDiff / (this.GetSize())) * NumFieldPoints;
        float z = (zDiff / (this.GetSize())) * NumFieldPoints;

        if (x >= NumFieldPoints){
            x = NumFieldPoints - 1;
        }
        if (x < 0.0f){
            x = 0.0f;
        }
        if (z >= NumFieldPoints){
            z = NumFieldPoints - 1;
        }
        if (z < 0.0f){
            z = 0.0f;
        }

        return new Vector2(x, z);
    }

    // InitializePoints initializes field points
    public WaterPoint[,] InitializePoints(int numPoints) {
        //initialize all the points without neighbours
        int counter = 0;
        WaterPoint[,] points = new WaterPoint[numPoints,numPoints];

        for (int i = 0; i < numPoints;i++){
            for (int j = 0; j < numPoints; j++){
                counter++;
                points[i,j] = new WaterPoint(this, 0.0f);
            }
        }
        //now that we have initialized all our points, lets set all their neighbours
        //we have each point "connected" to adjascent points so that we can cascade physic effects
        //over the matrix
        for (int i = 0; i < numPoints;i++){
            for (int j = 0; j < numPoints; j++){
                List<WaterPoint> neighbours = new List<WaterPoint>();
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
                points[i,j].SetNeighbours(neighbours.ToArray());
            }
        }
        return points;
    }

}