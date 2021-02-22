using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectFind;

public class waterParticles : MonoBehaviour
{


    public float gravity = 0.00f;
    private int count = 0;

    public GameObject waterSpout;

    public GameObject waterController;

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

        //initialize water
        waterController = FindFirstWithTag("GameController");

    }

    void LateUpdate()
    {

        //get the acceleration for each particle (stored as custom data)
        system.GetCustomParticleData(customDat, ParticleSystemCustomData.Custom1);

        int numParticlesAlive = system.GetParticles(m_Particles);
        potController controller = waterController.GetComponent<potController>();
        waterPipe waterPipe = waterSpout.GetComponent<waterPipe>();
        int count = 0;
        Vector3 positionSum = new Vector3(0.0f,0.0f,0.0f);
        for (int i = 0; i < numParticlesAlive; i++)
        {
            float acceleration = customDat[i].x;
            //add the acceleration to the velocity
            m_Particles[i].position += Vector3.up*acceleration;
            //increase the accleration using gravity
            acceleration -= gravity;
            float underwater = customDat[i].y;
            if (controller.GetWaterMaxHeight() + 
                controller.GetWaterPosition().y >= m_Particles[i].position.y
                && underwater == 0.0f){
                //we are also going to broadcast to the water spout at what position we hit the water
                positionSum+=m_Particles[i].position;
                count++;
                underwater = 1.0f;
            }

            //store the acceleration and underwater status of the particle
            customDat[i] = new Vector4(acceleration,underwater,0.0f,0.0f);
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
    }
}
