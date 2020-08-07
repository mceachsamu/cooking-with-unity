using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class point
{
    //position of point in world
    public float y;

    //the neighbouring points
    public  point[] neighbours;

    //how much neighbouring points effect this point

    public float curDeceleration = -0.00f;

    public float acceleration = 0.0f;

    private potwater water;

    public float frictionForce;
    public float forceApplied;

    public float speed = 0.0f;


    //initialize our point
    public point(potwater waterObj, float y){
        this.water = waterObj;
        this.y = y;
    }

        //move this point based on its speed and its neighbours position
    public void move(){
        //determine the direction of the center and set curDecelleration towards that direction

        float totalForce = 0.0f;
        for (int i = 0; i < neighbours.Length;i++){
            float difference = neighbours[i].y - this.y;
            totalForce += difference;
        }
        this.forceApplied = (totalForce / neighbours.Length) *  water.neighbourFriction;

        this.frictionForce = 0.5f * Mathf.Pow(this.acceleration,2.0f) * water.friction;


        //decrease/increase gravity based on amplitude
        if (this.y > 0){
            this.curDeceleration = 1.0f * (water.deceleration * 1.0f * Mathf.Abs(this.y) * water.damping);
        }
        if (this.y < 0){
            this.curDeceleration = -1.0f * (water.deceleration * 1.0f * Mathf.Abs(this.y) * water.damping);
        }

        this.forceApplied += this.curDeceleration;
        this.addForce(this.forceApplied);
        this.speed += this.acceleration;
        this.acceleration = this.acceleration * water.drag;
        this.y += this.acceleration;
    }

    public Vector4 GetHeightValue(){
        float height =  ((this.y + water.maxHeight))/(water.maxHeight*2.0f);
        return new Vector4(height,height,height,1.0f);
    }
    public void addForce(float force){
        this.acceleration += force/water.mass;
    }

    //set the neighbours for this point
    public void setNeighbours(point[] neighs){
        this.neighbours = neighs;
    }

}
