using NUnit.Framework;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Game_State Current_State;
    private Game_State Previous_State;

    public int WaveCount;
    public int CurrentWaveIndex;

    [Header("절대불변")]
    public PlayerController Player;
    public SpawnManagers spawnManagers;
    public PlayerInventoryManager Inventory;

    [Header("맵마다 변경")]
    public Transform Core;
    public EnemySpawnManager CurrentMap;

    private void Awake()
    {
        Instance = this;
        Current_State = Game_State.GameInit;
    }

    private void Update()
    {
        CheckState();
    }

    public void Init()
    {
        spawnManagers.Init();
        CurrentMap.Player = Player;
        CurrentMap.Core = Core;
        Inventory.InitInventory();
        Player.PlayerInit(CurrentMap.PlayerSpawnPoint.position);
        Current_State = Game_State.MapInit;
    }

    void CheckState()
    {
        switch (Current_State)
        {
            case Game_State.GameInit:
                Init();
                break;
            case Game_State.MapInit:
                MapInit();
                break;
        }
    }

    public void MapInit()
    {
        CurrentMap.MapInit();
        Current_State = Game_State.Attack;
    }

    public void ReadyWave()
    {
        CurrentMap.ReadyWave();
        Current_State = Game_State.Ready;
    }

    public void CheckWaveEnd()
    {
        if (spawnManagers.Enemy.EnemyCount==0&&CurrentMap.IsSpawnEnd())
        {
            if(WaveCount < CurrentWaveIndex)
            {
                WaveCount++;
                ReadyWave();
            }
            else
            {
                //맵 클리어
                MapEnding();
            }
        }
    }

    public void CheckAttackEnd()
    {
        if(CurrentMap.DefaultEnemyCount == 0)
        {
            ReadyWave();
            Current_State = Game_State.Ready;
        }
    }

    public void WaveStart()
    {
        CurrentMap.StartWave();
        Current_State = Game_State.Waving;
    }

    public void MapEnding()
    {
        Current_State = Game_State.MapEnd;
    }
}

public enum Game_State
{
    GameInit,
    MapInit,
    Attack,
    Ready,
    Waving,
    MapEnd
}