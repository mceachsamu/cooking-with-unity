﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterParticles : MonoBehaviour
{


    public float gravity = 0.00f;
    private int count = 0;

    public GameObject waterSpout;

    public GameObject water;

    public GameObject bottleEnd;

    Vector3 LastValidPosition = new Vector3(0.0f,0.0f,0.0f);

    ParticleSystem system;
    ParticleSystem.Particle[] m_Particles;

    List<Vector4> customDat = new List<Vector4>();

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = bottleEnd.GetComponent<Transform>().position;
        system = this.GetComponent<ParticleSystem>();
        m_Particles = new ParticleSystem.Particle[system.main.maxParticles];
    }

    void LateUpdate()
    {

        //get the acceleration for each particle (stored as custom data)
        system.GetCustomParticleData(customDat, ParticleSystemCustomData.Custom1);

        int numParticlesAlive = system.GetParticles(m_Particles);
        potwater potwater = water.GetComponent<potwater>();
        waterPipe waterPipe = waterSpout.GetComponent<waterPipe>();
        int count = 0;
        Vector3 positionSum = new Vector3(0.0f,0.0f,0.0f);
        for (int i = 0; i < numParticlesAlive; i++)
        {
            float acceleration = customDat[i].x;
            //add the acceleration to the velocity
            //we first need to transform the position of the paricle into world space
            Vector3 pWpos = this.transform.TransformPoint(m_Particles[i].position);
            //we can adjust this position to tend towards the ground at a rate of its acceleration
            pWpos += Vector3.up*acceleration;
            //transform the position back to local space and set the paricles position
            m_Particles[i].position = this.transform.InverseTransformPoint(pWpos);
            //increase the accleration using gravity
            acceleration -= gravity;
            customDat[i] = new Vector4(acceleration,0.0f,0.0f,0.0f);
            if (potwater.getHeightAtPosition(pWpos) >= pWpos.y){
                //we are also going to broadcast to the water spout at what position we hit the water
                positionSum+=pWpos;
                count++;
                //m_Particles[i].remainingLifetime = 0.0f;
                //customDat[i] = new Vector4(0.0f,0.0f,0.0f,0.0f);
            }
        }
        if (count != 0){
            LastValidPosition = positionSum/count;
            waterPipe.SetFallPosition(LastValidPosition);
        }else {
            waterPipe.SetFallPosition(LastValidPosition);
        }

        system.SetParticles(m_Particles, numParticlesAlive);

        system.SetCustomParticleData(customDat, ParticleSystemCustomData.Custom1);

        count++;
       //this.GetComponent<Renderer>().material.SetFloat("_Count", count);
       //this.GetComponent<Renderer>().material.SetVector("_LightPos", Light.transform.position);

    }
}