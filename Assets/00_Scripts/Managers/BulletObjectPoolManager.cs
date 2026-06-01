using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObjectPoolManager : MonoBehaviour
{
    public Queue<BulletController> BulletPool;
    public int DefaultBulletPoolCount = 30;

    public Transform Bullets;

    public BulletController BulletObject;

    public void BulletPoolInit()
    {
        BulletPool = new Queue<BulletController>();
        for (int i = 0; i < DefaultBulletPoolCount; i++)
        {
            BulletController bullet = spawnbullet();
            BulletPool.Enqueue(bullet);
        }
    }

    public void SpawnBullet(Vector3 Position, Quaternion Rotation, float BulletSpeed, int BulletCount ,float BulletDelay)
    {
        StartCoroutine(SpawningBullets(Position, Rotation, BulletSpeed, BulletCount, BulletDelay));
    }

    IEnumerator SpawningBullets(Vector3 Position, Quaternion Rotation, float BulletSpeed, int BulletCount, float BulletDelay)
    {
        for(int i = 0; i < BulletCount; i++)
        {
            BulletController bullet;
            if (BulletPool.Count > 0)
            {
                bullet = BulletPool.Dequeue();
            }
            else
            {
                bullet = spawnbullet();
            }

            bullet.BulletInit(Position, Rotation, BulletSpeed);

            yield return new WaitForSeconds(BulletDelay);
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
