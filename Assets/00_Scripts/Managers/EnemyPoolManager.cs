using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class EnemyPoolManager : MonoBehaviour
{
    [Header("적 원본 모델")]
    public EnemyController[] Enemies;

    [Header("적별 소환위치 부모")]
    public Transform EnemyPoolsParent;

    [Header("적별 소환위치")]
    public Transform[] EnemyParents;

    [Header("적별 폴링 수")]
    public int[] DefulatSpawnCounts;

    [Header("적별 폴링 리스트")]
    public Queue<EnemyController>[] EnemyPools;

    public List<EnemyController> SpawnEnemys;

    [Header("적의 수")]
    public int EnemyCount=0;

    public void Init()
    {
        EnemyParents = new Transform[Enemies.Length];

        EnemyPools = new Queue<EnemyController>[Enemies.Length];

        for (int i = 0; i < EnemyParents.Length; i++)
        {
            EnemyParents[i] = new GameObject().transform;
            EnemyParents[i].parent = EnemyPoolsParent;

            EnemyParents[i].name = ((Enemy_Type)i).ToString();
            EnemyPools[i] = new Queue<EnemyController>();

            for (int j = 0; j < DefulatSpawnCounts[i]; j++)
            {
                InPool(i, spawnEnemy(i));
            }
        }
    }
    
    public EnemyController SpawnEnemy(Enemy_Type Type)
    {
        int IntType = (int)Type;

        EnemyController Enemy;

        if (EnemyPools[IntType].Count > 0)
        {
            Enemy = EnemyPools[IntType].Dequeue();
        }
        else
        {
            Enemy = spawnEnemy(IntType);
        }

        EnemyCount++;
        SpawnEnemys.Add(Enemy);
        return Enemy;
    }
    
    public void InPool(int EnemyNumber, EnemyController Enemy)
    {
        EnemyPools[EnemyNumber].Enqueue(Enemy);
        if (SpawnEnemys.Contains(Enemy))
        {
            SpawnEnemys.Remove(Enemy);
        }
    }

    public void ImDead()
    {
        EnemyCount--;
    }

    EnemyController spawnEnemy(int type)
    {
        EnemyController enemy = Instantiate(Enemies[type], EnemyParents[type]).GetComponent<EnemyController>();
    
        enemy.gameObject.SetActive(false);

        enemy.DefaultInit(this, (Enemy_Type)type);

        return enemy;
    }

    public void AllBack()
    {
        for(int i = 0; i < SpawnEnemys.Count;)
        {
            EnemyController enemy = SpawnEnemys[i];
            enemy.Back();
        }
        EnemyCount = 0;
    }
}

public enum Enemy_Type 
{
    Normal,
    ShotGun,
    CoreRush,
    Stealth,
    Wall
}