using System.Collections.Generic;
using UnityEngine;

public class ParticlesSpawnManager : MonoBehaviour
{
    [Header("파티클 원본 모델")]
    public ParticleController[] Particles;

    [Header("파티클 소환위치 부모")]
    public Transform ParticlePoolsParent;

    [Header("파티클 소환위치")]
    public Transform[] ParticleParents;

    [Header("파티클 폴링 수")]
    public int[] DefulatSpawnCounts;

    [Header("파티클별 폴링 리스트")]
    public Queue<ParticleController>[] ParticlePools;


    public void Init()
    {
        ParticleParents = new Transform[Particles.Length];

        ParticlePools = new Queue<ParticleController>[Particles.Length];

        for(int i = 0; i < ParticleParents.Length; i++)
        {
            ParticleParents[i] = new GameObject().transform;
            ParticleParents[i].parent = ParticlePoolsParent;

            ParticleParents[i].name = ((Particle_Type)i).ToString();

            ParticlePools[i] = new Queue<ParticleController>();

            for (int j = 0; j < DefulatSpawnCounts[i]; j++)
            {
                InPool(i, spawnParticle(i));
            }
        }
    }

    public ParticleController SpawnParticle(Particle_Type Type, Vector3 Position, Quaternion Rotation)
    {
        int IntType = (int)Type;

        ParticleController Particle;

        if (ParticlePools[IntType].Count > 0)
        {
            Particle = ParticlePools[IntType].Dequeue();
        }
        else
        {
            Particle = spawnParticle(IntType);
        }

        Particle.Init(Position, Rotation);

        return Particle;
    }

    public void InPool(int ParticleNumber, ParticleController Particle)
    {
        ParticlePools[ParticleNumber].Enqueue(Particle);
    }

    ParticleController spawnParticle(int type)
    {
        ParticleController particle = Instantiate(Particles[type].gameObject, ParticleParents[type]).GetComponent<ParticleController>();

        particle.gameObject.SetActive(false);

        particle.DefaulInit(this, type);

        return particle;
    }
}

public enum Particle_Type
{
    BulletParring,
    SwordParring,
    EnemyHit,
    Playerhit
}