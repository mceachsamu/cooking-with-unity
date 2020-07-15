using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class point
{
    //position of point in world
    public float y;

    public float x;

    public float z;
    //the neighbouring points
    public  point[] neighbours;

    //how much neighbouring points effect this point

    private float mass = 3.0f;

    private float deceleration = -0.1f;
    public float curDeceleration = -0.00f;

    public float drag = 0.97f;
    public float acceleration = 0.0f;

    public float speed = 0.0f;

    public float neighbourFriction = 0.1f;
    private float friction = 0.8f;
    public float frictionForce;
    public float forceApplied;

    private float maxHeight = 1.0f;

    public Vector4 GetHeightValue(){
        float height =  ((this.y + maxHeight))/(maxHeight*2.0f);
        return new Vector4(height,height,height,1.0f);
    }
    public void addForce(float force){
        this.acceleration += force/mass;
    }

    //initialize our point
    public point(float x, float y, float z){
        this.x = x;
        this.y = y;
        this.z = z;
    }
    //set the neighbours for this point
    public void setNeighbours(point[] neighs){
        this.neighbours = neighs;
    }

    public float getMomentum(){
        return mass * speed;
    }

    //move this point based on its speed and its neighbours position
    public void move(){
        if (this.y > 0){
            this.curDeceleration = 1.0f * this.deceleration;
        }
        if (this.y < 0){
            this.curDeceleration = -1.0f * this.deceleration;
        }
        float totalForce = 0.0f;
        for (int i = 0; i < neighbours.Length;i++){
            float difference = neighbours[i].y - this.y;
            totalForce += difference;
        }
        this.forceApplied = (totalForce / neighbours.Length) * neighbourFriction;
        
        this.frictionForce = 0.5f * Mathf.Pow(this.acceleration,2.0f) * friction;
        float frictionDirection = 1.0f;
        if (this.speed < 0.0f){
            frictionDirection = -1.0f;
        }
        if (this.speed > 0.0f){
            frictionDirection = 1.0f;
        }
        //apply friction
        this.forceApplied += (-this.frictionForce * frictionDirection);
        if (Mathf.Abs(this.forceApplied) < this.curDeceleration + 0.5f){
            this.curDeceleration = this.curDeceleration * 0.001f;
        }
        //apply gravity
        this.forceApplied += this.curDeceleration;
        this.addForce(this.forceApplied);
        this.speed += this.acceleration;
        this.acceleration = this.acceleration * drag;
        this.y += this.acceleration;
    }

}
