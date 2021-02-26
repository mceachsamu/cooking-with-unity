using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is a class used to generate 3d shapes on program startup
public class shapes3D
{

    //creates a simple plane mesh to be used as the water surface
    public Mesh CreatePlane(int numSegs, float segSize){
        Mesh m = new Mesh();
        m.name = "mesh";
        m.Clear();

        float size = numSegs * segSize;

        Vector3[] vs = new Vector3[(int)(numSegs*numSegs)*6];
        Vector3[] nm = new Vector3[(int)(numSegs*numSegs)*6];
        Vector2[] us = new Vector2[(int)(numSegs*numSegs)*6];
        int[] tri = new int[(int)(numSegs*numSegs)*6];

        float width = (float)(numSegs) * segSize;
        int count = 0;
            for (int i = 0; i < numSegs; i++){
                for (int j = 0; j < numSegs; j++){
                //first traingle
                vs[count] = new Vector3(i*segSize - size/2.0f ,0.0f, j*segSize- size/2.0f);
                us[count] = new Vector2( (float)i / (float)numSegs, (float)j /(float)numSegs);
                tri[count] = count;
                count++;

                vs[count] = new Vector3(i*segSize - size/2.0f,0.0f,(j*segSize + segSize) - size/2.0f);
                us[count] = new Vector2( ((float)i) / (float)numSegs,  ((float)j+1.0f) / (float)numSegs);
                tri[count] = count;
                count++;

                vs[count] = new Vector3(i*segSize + segSize - size/2.0f,0.0f,j*segSize - size/2.0f);
                us[count] =  new Vector2( ((float)i+1.0f) / (float)numSegs,  ((float)j) / (float)numSegs);
                tri[count] = count;
                count++;

                //second triangle
                vs[count] = new Vector3(i*segSize + segSize - size/2.0f,0.0f,j*segSize - size/2.0f);
                us[count] = new Vector2( ((float)i+1.0f) / (float)numSegs,  ((float)j) / (float)numSegs);
                tri[count] = count;
                count++;

                vs[count] = new Vector3(i*segSize - size/2.0f,0.0f,j*segSize + segSize - size/2.0f);
                us[count] = new Vector2(((float)i) / (float)numSegs,  ((float)j+1.0f) / (float)numSegs);
                tri[count] = count;
                count++;

                vs[count] = new Vector3(i*segSize + segSize - size/2.0f,0.0f,j*segSize + segSize - size/2.0f);
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

    public Mesh CreateCylandar(float radius, float length, int numSegsRound, int numSegsLong) {
        Mesh m = new Mesh();
        m.name = "mesh";
        m.Clear();

        Vector3[] vs = new Vector3[(int)(numSegsLong*numSegsRound)*6];
        Vector2[] us = new Vector2[(int)(numSegsLong*numSegsRound)*6];
        int[] tri = new int[(int)(numSegsLong*numSegsRound)*6];

        int count = 0;
        for (int i = 0; i < numSegsLong; i++){
            for (int j = 0; j < numSegsRound; j++){
                float angle = (Mathf.PI*2 / numSegsRound) * j;
                float angle2 = (Mathf.PI*2 / numSegsRound) * (j + 1);

                float z = (length / numSegsLong) * i;
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;

                float z2 = (length / numSegsLong) * (i+1);
                float x2 = Mathf.Cos(angle2) * radius;
                float y2 = Mathf.Sin(angle2) * radius;

                vs[count] = new Vector3(x ,y, z);
                us[count] = new Vector2((float)j / (float)numSegsRound, (float)i / (float)numSegsLong);
                tri[count] = count;
                count++;

                vs[count] = new Vector3(x2 ,y2, z);
                us[count] = new Vector2((float)(j+1) / (float)numSegsRound, (float)i / (float)numSegsLong);
                tri[count] = count;
                count++;

                vs[count] = new Vector3(x2 ,y2, z2);
                us[count] = new Vector2((float)(j+1) / (float)numSegsRound, (float)(i + 1)/ (float)numSegsLong);
                tri[count] = count;
                count++;

                vs[count] = new Vector3(x2 ,y2, z2);
                us[count] = new Vector2((float)(j+1) / (float)numSegsRound, (float)(i + 1)/ (float)numSegsLong);
                tri[count] = count;
                count++;

                vs[count] = new Vector3(x ,y, z2);
                us[count] = new Vector2((float)j / (float)numSegsRound, (float)(i + 1)/ (float)numSegsLong);
                tri[count] = count;
                count++;

                vs[count] = new Vector3(x ,y, z);
                us[count] = new Vector2((float)j / (float)numSegsRound, (float)i/ (float)numSegsLong);
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
