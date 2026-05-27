using System.Collections.Generic;
using UnityEngine;

public class BulletObjectPoolManager : MonoBehaviour
{
    public static BulletObjectPoolManager instance;

    public Queue<BulletController> BulletPool;
    public int DefaultBulletPoolCount = 30;

    public Transform Bullets;

    public BulletController BulletObject;

    private void Awake()
    {
        instance = this;
    }

    public void BulletPoolInit()
    {
        BulletPool = new Queue<BulletController>();
        for (int i = 0; i < DefaultBulletPoolCount; i++)
        {
            BulletController bullet = spawnbullet();
            BulletPool.Enqueue(bullet);
        }
    }

    public void SpawnBullet(Vector3 Position, Quaternion Rotation, float BulletSpeed)
    {
        if(BulletPool.Count > 0)
        {
            BulletController bullet = BulletPool.Dequeue();

            bullet.BulletInit(Position, Rotation, BulletSpeed);
        }
        else
        {
            BulletController bullet = spawnbullet();

            bullet.BulletInit(Position, Rotation, BulletSpeed);
        }
    }

    BulletController spawnbullet()
    {
        BulletController bullet = Instantiate(BulletObject.gameObject).GetComponent<BulletController>();
        bullet.gameObject.SetActive(false);
        bullet.SetBullet(this, Bullets);
        return bullet;
    }
}
