using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
public class potwater : MonoBehaviour
{
    //the prefab for the bubbles
    public GameObject prefab;
    //the pot that the water is sitting in
    public GameObject pot;
    //the light source of the scene
    //TODO implement multiple light sources
    public GameObject Light;
    //the object who's transformation defines where the water can be seen through
    public GameObject lid;
    //the game object that is defined at the bottom of the spoon
    public GameObject SpoonHead;

    //radius of the potlid on bot x and z directions
    //TODO implement zRadius to allow oval lids
    //the radius of the lid that we render inside
    public float xRadConst;
    public float zRadConst;

    //the number of segments the plane is made up from
    public int numSegs = 10;
    //the size of the segments on the plane generated
    public float segSize = 5.0f;


    float count = 0.0f;

    //the speed the bubbles pop
    [Range(0.0f, 0.5f)]
    public float speed = 0.1f;
    //the number of bubbles that appear on the water
    public int numBubbles = 20;
    //the list of bubble game objects
    private GameObject[] bubbles;

    //the main color of the water
    private Vector4 primaryCol = new Vector3(3.0f/255,71.0f/255.0f,105.0f/255.0f);
    //the shading color of the water
    private Vector4 secondaryCol = new Vector3(255.0f/255.0f,137.0f/255.0f,0.0f/255.0f);
    //the number of points defining the phsyical water mesh (complexity of (numFieldPoints ^ 2)* 4)
    public int numFieldPoints = 10;
    //the physical water mesh points, these define the y positions of the water surface
    private point[,] pointField;

    //the resulting texture from the points field
    public Texture2D heightMap;

    //record the previous spoon position on each frame, this is to get a differential if the spoon position
    private Vector3 preSpoonPos;
    void Start()
    {
       // ClearLog();
        //get the radius of our pot lid
        xRadConst = lid.GetComponent<lid>().lidXradius;
        zRadConst = lid.GetComponent<lid>().lidZradius;
        //set the same as xradius for now
        //create a new 2d plane mesh for this object
        Mesh mesh = createPlane(numSegs, segSize);
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
    }


    public void AddForceToWater(Transform transform, float forceAmount){
        //ensures we dont get an out of bounds exception and translates position to water
        Vector2 index = getClosestPoint(transform);
        pointField[(int)index.x,(int)index.y].addForce(-1 * forceAmount);
    }


    // Update is called once per frame
    void Update()
    {

        //count is an internal timer we give to the shader to move the water along with time
        count = count + speed;
        for (int i = 0; i < numFieldPoints;i++){
            for (int j = 0; j < numFieldPoints;j++){
                pointField[i,j].move();
                //points[i,j].GetComponent<Transform>().position = new Vector3(pointField[i,j].x, pointField[i,j].y, pointField[i,j].z);
                heightMap.SetPixel(i,j, pointField[i,j].GetHeightValue());
            }
        }

        heightMap.Apply();

        //radius of the shape will be the mesh size * the scaling of the object
        float xRad = pot.transform.localScale.x * xRadConst;
        float zRad = pot.transform.localScale.z * zRadConst;
        Vector3 center = lid.transform.position;
        //get the center point for the pot
        Vector4 center2 = new Vector4(center.x,center.y,center.z,0.0f);

        Vector3 spoonEnd = SpoonHead.transform.position;
        Vector4 spoonEnd2 = new Vector4(spoonEnd.x,spoonEnd.y,spoonEnd.z,0.0f);
        //give these values to our shader
        this.GetComponent<Renderer>().material.SetFloat("xRad", xRad);
        this.GetComponent<Renderer>().material.SetFloat("zRad", zRad);
        this.GetComponent<Renderer>().material.SetFloat("seperation", segSize);
        this.GetComponent<Renderer>().material.SetFloat("totalSize", segSize * numSegs);
        this.GetComponent<Renderer>().material.SetVector("center", center2);
        this.GetComponent<Renderer>().material.SetTexture("_MainTex", heightMap);
        this.GetComponent<Renderer>().material.SetVector("baseColor", primaryCol);
        this.GetComponent<Renderer>().material.SetVector("secondaryColor", secondaryCol);
        this.GetComponent<Renderer>().material.SetVector("_LightPos", Light.transform.position);
        //for each bubble, set the properties
        for (int i = 0; i < numBubbles;i++){
            Vector2 bubbleIndex = getClosestPoint(bubbles[i].transform);
            float bubbleHeight = heightMap.GetPixel((int)bubbleIndex.x, (int)bubbleIndex.y).r;
            bubbles[i].transform.position.Set(bubbles[i].transform.position.x, bubbles[i].transform.position.y+bubbleHeight, bubbles[i].transform.position.z);
            bubbles[i].GetComponent<Renderer>().material.SetTexture("_MainTex", heightMap);
            bubbles[i].GetComponent<bubble>().zRadius = zRad;
            bubbles[i].GetComponent<bubble>().xRadius = xRad;
            bubbles[i].GetComponent<bubble>().scaleIncrease = speed;
            bubbles[i].GetComponent<bubble>().center = new Vector3(pot.transform.position.x, this.transform.position.y+bubbleHeight, pot.transform.position.z);
            bubbles[i].GetComponent<Renderer>().material.SetFloat("xRad", xRad);
            bubbles[i].GetComponent<Renderer>().material.SetFloat("zRad", zRad);
            bubbles[i].GetComponent<Renderer>().material.SetFloat("time", count);
            bubbles[i].GetComponent<Renderer>().material.SetFloat("waterSize", segSize * numSegs);
            bubbles[i].GetComponent<Renderer>().material.SetVector("center", center2);
            bubbles[i].GetComponent<Renderer>().material.SetVector("spoon_end", spoonEnd2);
            bubbles[i].GetComponent<Renderer>().material.SetVector("_LightPos", Light.transform.position);
            bubbles[i].GetComponent<Renderer>().material.SetVector("_Color", primaryCol);
            bubbles[i].GetComponent<Renderer>().material.SetVector("_Color2", secondaryCol);
        }
        //set the previous spoon position
        preSpoonPos = SpoonHead.transform.position;
    }

    //creates a simple plane mesh to be used as the water surface
    public Mesh createPlane(int numSegs, float segSize){
        Mesh m = new Mesh();
        m.name = "mesh";
        m.Clear();

        Vector3[] vs = new Vector3[(int)(numSegs*numSegs)*6];
        Vector2[] us = new Vector2[(int)(numSegs*numSegs)*6];
        int[] tri = new int[(int)(numSegs*numSegs)*6];

        float width = (float)(numSegs) * segSize;
        int count = 0;
            for (int i = 0; i < numSegs; i++){
                for (int j = 0; j < numSegs; j++){
                //first traingle
                vs[count] = new Vector3(i*segSize,0.0f,j*segSize);
                us[count] = new Vector2( (float)i / (float)numSegs, (float)j /(float)numSegs);
                tri[count] = count;
                count++;

                vs[count] = new Vector3(i*segSize,0.0f,j*segSize + segSize);
                us[count] = new Vector2( ((float)i) / (float)numSegs,  ((float)j+1.0f) / (float)numSegs);
                tri[count] = count;
                count++;

                vs[count] = new Vector3(i*segSize + segSize,0.0f,j*segSize);
                us[count] =  new Vector2( ((float)i+1.0f) / (float)numSegs,  ((float)j) / (float)numSegs);
                tri[count] = count;
                count++;

                //second triangle
                vs[count] = new Vector3(i*segSize + segSize,0.0f,j*segSize);
                us[count] = new Vector2( ((float)i+1.0f) / (float)numSegs,  ((float)j) / (float)numSegs);
                tri[count] = count;
                count++;

                vs[count] = new Vector3(i*segSize,0.0f,j*segSize + segSize);
                us[count] = new Vector2(((float)i) / (float)numSegs,  ((float)j+1.0f) / (float)numSegs);
                tri[count] = count;
                count++;

                vs[count] = new Vector3(i*segSize + segSize,0.0f,j*segSize + segSize);
                us[count] = new Vector2( ((float)i + 1.0f) / (float)numSegs,  ((float)j + 1.0f) / (float)numSegs);
                tri[count] = count;
                count++;
            }
        }
        m.vertices = vs;
        m.uv = us;
        m.triangles = tri;
        m.RecalculateNormals();
        return m;
    }
    //translates wolrd positions into the closest index on the field points matrix.
    private Vector2 getClosestPoint(Transform transform){
        float xDiff = transform.position.x - this.transform.position.x;
        float zDiff = transform.position.z - this.transform.position.z;
        float x = (xDiff / (segSize * numSegs)) * numFieldPoints;
        float z = (zDiff / (segSize * numSegs)) * numFieldPoints;
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
                points[i,j] = new point(0.0f);
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
        bub.GetComponent<MeshRenderer>().material = prefab.GetComponent<MeshRenderer>().material;
        bub.GetComponent<MeshFilter>().mesh = prefab.GetComponent<MeshFilter>().mesh;
        bub.AddComponent<bubble>();
        bub.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
        bub.GetComponent<bubble>().center = new Vector3(pot.transform.position.x, lid.transform.position.y, pot.transform.position.z);
        bub.GetComponent<Renderer>().material.SetVector("baseColor", primaryCol);
        bub.GetComponent<Renderer>().material.SetVector("secondaryColor", secondaryCol);
        bub.GetComponent<bubble>().scaleIncrease = Random.Range(0.003f,0.006f);
        bub.GetComponent<bubble>().maxScale = Random.Range(0.05f,0.35f);
        bub.GetComponent<bubble>().water = this;
        bub.GetComponent<Transform>().parent = this.GetComponent<Transform>();
        return bub;
    }

}