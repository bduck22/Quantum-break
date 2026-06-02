using NUnit.Framework;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Never Changed")]
    public PlayerController Player;
    public SpawnManagers spawnManagers;

    [Header("Changed In OneMap")]
    public Transform Core;
    public EnemySpawnManager CurrentMap;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        spawnManagers.Init();
        MapStart();
    }

    public void MapStart()
    {
        CurrentMap.Player = Player;
        CurrentMap.Core = Core;
        CurrentMap.MapInit();
    }
}
