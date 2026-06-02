using UnityEngine;

public class SpawnManagers : MonoBehaviour
{
    public static SpawnManagers Instance;

    public BulletObjectPoolManager Bullet;
    public ParticlesSpawnManager Particle;
    public EnemyPoolManager Enemy;

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        Bullet.BulletPoolInit();
        Particle.Init();
        Enemy.Init();
    }
}
