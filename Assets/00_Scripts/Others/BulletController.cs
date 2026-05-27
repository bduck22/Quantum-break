using UnityEngine;

public class BulletController : MonoBehaviour
{
    BulletObjectPoolManager poolManager;

    public float BulletSpeed;

    public void SetBullet(BulletObjectPoolManager poolManager, Transform Parent)
    {
        this.poolManager = poolManager;

        transform.parent = Parent;
    }

    public void BulletInit(Vector3 Position, Quaternion Rotation, float BulletSpeed)
    {
        this.BulletSpeed = BulletSpeed;

        transform.position = Position;
        transform.rotation = Rotation;

        gameObject.SetActive(true);
    }

    public void OnFalse()
    {
        transform.gameObject.SetActive(false);
        poolManager.BulletPool.Enqueue(this);
    }
}
