using NUnit.Framework;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("절대불변")]
    public PlayerController Player;
    public SpawnManagers spawnManagers;

    [Header("맵마다 변경")]
    public Transform Core;
    public EnemySpawnManager CurrentMap;

    private void Awake()
    {
        Instance = this;
    }

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
