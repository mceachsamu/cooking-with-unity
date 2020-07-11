using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
public class potwater : MonoBehaviour
{

    public GameObject prefab;
    public GameObject prefab2;
    public GameObject pot;
    public GameObject Light;
    public GameObject lid;

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

    //the speed the water water swirles about
    [Range(0.0f, 0.5f)]
    public float speed = 0.1f;

    public int numBubbles = 200;

    private GameObject[] bubbles;


    private Vector4 primaryCol = new Vector3(1.0f,0.349f,0.0f);
    private Vector4 secondaryCol = new Vector3(1.0f,0.643f,0.450f);
    // Start is called before the first frame update
    public int numFieldPoints = 10;
    public float speration = 1;
    private point[,] pointField;
    private GameObject[,] points;

    public Texture2D heightMap;
    void Start()
    {
       // ClearLog();
        //get the radius of our pot lid
        xRadConst = lid.GetComponent<lid>().lidXradius;
        zRadConst = lid.GetComponent<lid>().lidZradius;
        //set the same as xradius for now
        //create a new 2d plane mesh for this object
        //create a new 2d plane mesh for this object
        Mesh mesh = createPlane(numSegs, segSize);
        this.GetComponent<MeshFilter>().mesh = mesh;
        this.pointField = initializePoints(numFieldPoints, speration);
        // = visualizePoints(pointField);

        //initilize our bubbles
        bubbles = new GameObject[numBubbles];
        for (int i = 0; i < numBubbles;i++){
            bubbles[i] = createBubble(i);
        }
        pointField[4,4].addForce(6.0f);
        
        pointField[16,4].addForce(8.0f);
        heightMap = new Texture2D(numFieldPoints, numFieldPoints);
    }

    private point[,] initializePoints(int numPoints, float spacing) {
        //initialize all the points without neighbours
        int counter = 0;
        point[,] points = new point[numPoints,numPoints];
        for (int i = 0; i < numPoints;i++){
            for (int j = 0; j < numPoints; j++){
                counter++;
                points[i,j] = new point(i * spacing, 0, j * spacing);
            }
        }
        //now that we have initialized all our points, lets set all their neighbours
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

    public GameObject[,] visualizePoints(point[,] points) {
        int counter = 0;
        GameObject[,] objs = new GameObject[numFieldPoints,numFieldPoints];
        for (int i = 0; i < numFieldPoints;i++){
            for (int j = 0; j < numFieldPoints;j++){
                GameObject point = new GameObject("Point-" + i);
                point.AddComponent<MeshFilter>();
                point.AddComponent<MeshRenderer>();
                point.GetComponent<MeshRenderer>().material = prefab2.GetComponent<MeshRenderer>().material;
                point.GetComponent<MeshFilter>().mesh = prefab2.GetComponent<MeshFilter>().mesh;
                point.GetComponent<Transform>().localScale = prefab2.GetComponent<Transform>().localScale;
                point.GetComponent<Transform>().position = new Vector3(points[i,j].x, points[i,j].y, points[i,j].z);
                objs[i,j] = point;
                counter++;
            }
        }
        return objs;
    }

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
        return bub;
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
            bubbles[i].GetComponent<bubble>().zRadius = zRad;
            bubbles[i].GetComponent<bubble>().xRadius = xRad;
            bubbles[i].GetComponent<bubble>().scaleIncrease = speed;
            bubbles[i].GetComponent<bubble>().center = new Vector3(pot.transform.position.x, this.transform.position.y, pot.transform.position.z);
            bubbles[i].GetComponent<Renderer>().material.SetFloat("xRad", xRad);
            bubbles[i].GetComponent<Renderer>().material.SetFloat("zRad", zRad);
            bubbles[i].GetComponent<Renderer>().material.SetFloat("time", count);
            bubbles[i].GetComponent<Renderer>().material.SetVector("center", center2);
            bubbles[i].GetComponent<Renderer>().material.SetVector("spoon_end", spoonEnd2);
            bubbles[i].GetComponent<Renderer>().material.SetVector("_LightPos", Light.transform.position);
            bubbles[i].GetComponent<Renderer>().material.SetVector("_Color", primaryCol);
            bubbles[i].GetComponent<Renderer>().material.SetVector("_Color2", secondaryCol);
        }
    }

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

}