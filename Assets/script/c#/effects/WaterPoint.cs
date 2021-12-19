using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// WaterPoint represents a point on the water grid. Used in the water physics simulation
public class WaterPoint
{
    //position of point in world
    public float y;

    //the neighbouring points
    public  WaterPoint[] Neighbours;

    //how much neighbouring points effect this point
    public float CurrentDeceleration = 0.00f;
    public float Acceleration = 0.0f;

    private Potwater _water;

    public float FrictionForce;
    public float ForceApplied;

    public float Speed = 0.0f;


    // WaterPoint initializes our point
    public WaterPoint(Potwater waterObj, float y){
        this._water = waterObj;
        this.y = y;
    }

        // Move this point based on its speed and its neighbours position
    public void Move(){
        // Calculate the force given from the neighbouring points on the water surface
        float totalForce = 0.0f;
        for (int i = 0; i < Neighbours.Length; i++){
            float difference = Neighbours[i].y - this.y;
            totalForce += difference;
        }
        
        // Calculate the average force from the neigbours and apply friction
        this.ForceApplied = (totalForce / Neighbours.Length) *  _water.NeighbourFriction;

        // Deceleration is the counter force. This provides elasticity to the water surface
        // Here we calculate the deceleration using the water amplitude.
        if (this.y > 0){
            this.CurrentDeceleration = 1.0f * (_water.Deceleration * Mathf.Abs(this.y) * _water.Damping);
        } else {
            this.CurrentDeceleration = -1.0f * (_water.Deceleration * Mathf.Abs(this.y) * _water.Damping);
        }

        this.ForceApplied += this.CurrentDeceleration;
        this.Acceleration += this.ForceApplied/_water.Mass;
        this.Acceleration = this.Acceleration * _water.Drag;
        this.y += this.Acceleration;
    }

    public Vector4 GetHeightValue(){
        float height =  ((this.y + _water.MaxHeight))/(_water.MaxHeight*2.0f);
        return new Vector4(height,height,height,1.0f);
    }

    public void AddForce(float force){
        this.Acceleration += force/_water.Mass;
    }

    public void SetNeighbours(WaterPoint[] neighbours){
        this.Neighbours = neighbours;
    }

}
