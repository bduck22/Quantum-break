using System;
using UnityEngine;

[Serializable]
public class ParticleController : MonoBehaviour
{
    public ParticlesSpawnManager spawnManager;

    public ParticleSystem[] Particle;

    public int ParticleNumber;

    public void Init(Vector3 Position, Quaternion Rotation)
    {
        transform.position = Position;
        transform.rotation = Rotation;
    }

    public void DefaulInit(ParticlesSpawnManager spawnManager, int number)
    {
        this.spawnManager = spawnManager;

        ParticleNumber = number;
    }

    public void Play()
    {
        foreach(ParticleSystem particle in Particle)
        {
            particle.Play();
        }
    }

    void OnParticleSystemStopped()
    {
        spawnManager.InPool(ParticleNumber, this);
    }
}
