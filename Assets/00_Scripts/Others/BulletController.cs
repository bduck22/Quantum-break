using System;
using UnityEngine;

[Serializable]
public class BulletController : MonoBehaviour
{
    public bool IsStop;

    BulletObjectPoolManager poolManager;

    public float BulletSpeed;

    public float DestroyDistance;

    Vector3 OriginPosition;

    public TrailRenderer trail;

    private void Update()
    {
        if (IsStop)
        {
            return;
        }

        Move();
    }

    public void Parring(Transform Dir)
    {
        //NewPos -= new Vector3(0,0.5f,0);
        //Vector3 dir = (transform.position - NewPos).normalized;

        Quaternion rotation = Dir.rotation;//Quaternion.LookRotation(Dir.);

        BulletInit(transform.position, rotation, BulletSpeed);

        ParticleController particle =  SpawnManagers.Instance.Particle.SpawnParticle(Particle_Type.BulletParring, transform.position, Quaternion.identity);

        particle.Play();

        SetLayer(11);
    }

    void SetLayer(int layer)
    {
        gameObject.layer = layer;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.layer = layer;
        }
    }

    public void Move()
    {
        transform.position += transform.forward * BulletSpeed * Time.deltaTime;
        if(Vector3.Distance(transform.position, OriginPosition) >= DestroyDistance)
        {
            OnFalse();
        }
    }

    public void SetBullet(BulletObjectPoolManager poolManager, Transform Parent)
    {
        this.poolManager = poolManager;

        transform.parent = Parent;
    }

    public void BulletInit(Vector3 Position, Quaternion Rotation, float BulletSpeed)
    {
        SetLayer(10);
        this.BulletSpeed = BulletSpeed;

        transform.position = Position;
        transform.rotation = Rotation;

        OriginPosition = transform.position;

        trail.Clear();

        gameObject.SetActive(true);
    }

    public void OnFalse()
    {
        transform.gameObject.SetActive(false);
        poolManager.BulletPool.Enqueue(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6 || other.gameObject.layer == 14)
        {
            ParticleController particle = SpawnManagers.Instance.Particle.SpawnParticle(Particle_Type.Playerhit, transform.position, Quaternion.identity);
            particle.Play();
            OnFalse();
        }
    }
}
