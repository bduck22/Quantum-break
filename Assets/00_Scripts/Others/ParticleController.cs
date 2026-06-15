using System;
using UnityEngine;

[Serializable]
public class ParticleController : MonoBehaviour
{
    public ParticlesSpawnManager spawnManager;

    public ParticleControl[] Particle;

    public int ParticleNumber;

    public bool NotPool;

    public void Init(Vector3 Position, Quaternion Rotation)
    {
        transform.position = Position;
        transform.rotation = Rotation;
    }

    public void InitWithTimer(Vector3 Position, Quaternion Rotation, float Duration, float Size)
    {
        transform.position = Position;
        transform.rotation = Rotation;

        Vector3 size = new Vector3(Size, Size, Size);

        foreach (ParticleControl particle in Particle)
        {
            particle.Particle.Stop();
            var mian = particle.Particle.main;

            if (particle.IsDuration)
            {
                mian.duration = Duration;
            }

            if (particle.IsLifeTime)
            {
                mian.startLifetime = Duration;
            }

            if (particle.IsSize)
            {
                particle.Particle.transform.localScale = size;
            }
        }
    } 

    public void DefaulInit(ParticlesSpawnManager spawnManager, int number)
    {
        this.spawnManager = spawnManager;

        ParticleNumber = number;
    }

    public void Play()
    {
        gameObject.SetActive(true);
        foreach (ParticleControl particle in Particle)
        {
            particle.Particle.Play();
        }
    }

    void OnParticleSystemStopped()
    {
        if(!NotPool) spawnManager.InPool(ParticleNumber, this);
        gameObject.SetActive(false);
    }
}

[Serializable]
public struct ParticleControl
{
    public bool IsDuration;
    public bool IsLifeTime;
    public bool IsSize;
    public ParticleSystem Particle;
}