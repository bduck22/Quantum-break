using System;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public EnemyRouteController[] Spawners;

    public Action OnWaveStart;
    public Action<int> OnWaveStartInt;

    public Transform PlayerSpawnPoint;

    [HideInInspector]
    public PlayerController Player;
    [HideInInspector]
    public Transform Core;

    public int DefaultEnemyCount;

    public void MapInit()
    {
        foreach (EnemyRouteController spawner in Spawners)
        {
            spawner.player = Player;
            spawner.Core = Core;
        }
    }

    public bool IsSpawnEnd()
    {
        foreach (EnemyRouteController spawner in Spawners)
        {
            if (!spawner.SpawnEnd)
            {
                return false;
            }
        }
        return true;
    }

    public void ReadyWave()
    {
        OnWaveStartInt?.Invoke(GameManager.Instance.CurrentWaveIndex);
    }

    public void StartWave()
    {
        OnWaveStart?.Invoke();
    }
}
