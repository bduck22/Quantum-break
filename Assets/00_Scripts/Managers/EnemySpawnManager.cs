using System;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public EnemyRouteController[] Spawners;

    public int WaveCount;
    public int CurrentWaveIndex;

    public Action OnWaveStart;
    public Action<int> OnWaveStartInt;


    [HideInInspector]
    public PlayerController Player;
    [HideInInspector]
    public Transform Core;

    public void MapStart()
    {
        foreach (EnemyRouteController spawner in Spawners)
        {
            spawner.player = Player;
            spawner.Core = Core;
        }
    }
}
