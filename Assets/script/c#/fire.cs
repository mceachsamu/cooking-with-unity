using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fire : MonoBehaviour
{


    public float gravity = 0.01f;

    public float force = 0.1f;

    public GameObject water;
    private int count = 0;

    public GameObject Light;

    ParticleSystem system;
    ParticleSystem.Particle[] m_Particles;

    List<Vector4> customDat = new List<Vector4>();

    // Start is called before the first frame update
    void Start()
    {
        system = this.GetComponent<ParticleSystem>();
        m_Particles = m_Particles = new ParticleSystem.Particle[system.main.maxParticles];
    }

    void LateUpdate()
    {

        //get the acceleration for each particle (stored as custom data)
        system.GetCustomParticleData(customDat, ParticleSystemCustomData.Custom1);
        int numParticlesAlive = system.GetParticles(m_Particles);
        for (int i = 0; i < numParticlesAlive; i++)
        {
            float acceleration = customDat[i].x;
            //add the acceleration to the velocity
            m_Particles[i].velocity += Vector3.down * acceleration;

            //reduce the accleration using gravity
            acceleration -= gravity;
            customDat[i] = new Vector4(acceleration,0.0f,0.0f,0.0f);

            //add force to the water if we collided with it
            float height = water.GetComponent<potwater>().getHeightAtPosition(m_Particles[i].position);
            if (height > m_Particles[i].position.y){
                water.GetComponent<potwater>().AddForceToWater(m_Particles[i].position, force * m_Particles[i].startSize);
                m_Particles[i].remainingLifetime = 0.0f;
            }
        }

        system.SetCustomParticleData(customDat, ParticleSystemCustomData.Custom1);
        system.SetParticles(m_Particles, numParticlesAlive);

        count++;
        this.GetComponent<Renderer>().material.SetFloat("_Count", count);
        this.GetComponent<Renderer>().material.SetVector("_LightPos", Light.transform.position);

    }
}
