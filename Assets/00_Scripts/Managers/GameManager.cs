using IWantGoHome.ScreenEffects;
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

    public bool Cleared;

    [Header("절대불변")]
    public PlayerController Player;
    public SpawnManagers spawnManagers;
    public PlayerInventoryManager Inventory;
    public UIWindow RewardUI;
    public UIWindow ResultUI;

    [Header("맵마다 변경")]
    public EnemySpawnManager CurrentMap;
    public MainCoreController Core;

    public int CurrentMapIndex=0;
    public int MaxMapIndex;

    public MapData Map1;
    public MapData Map2;

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
        Cleared = false;
        spawnManagers.Init();
        Inventory.InitInventory();
        Current_State = Game_State.MapInit;
        MaxMapIndex = GameDataManager.Instance.GetRandomRoomCount();
        Application.targetFrameRate = 120;
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
        MapData Mapdata = GameDataManager.Instance.GetMap(0);

        

        CurrentMap.Player = Player;
        CurrentMap.Core = Core.transform;
        Player.PlayerInit(CurrentMap.PlayerSpawnPoint.position);
        Core.Coreinit();

        CurrentMap.MapInit();
        Current_State = Game_State.Attack;
    }

    public void ReadyWave()
    {
        CurrentWaveIndex++;
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
                Cleared = true;
                if(CurrentMapIndex < MaxMapIndex)
                {
                    MapEnding();
                }
                else
                {
                    Clear();
                }
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

    public bool isAttackEnd()
    {
        if (CurrentMap.DefaultEnemyCount == 0)
        {
            return true;
        }
        return false;
    }

    public void WaveStart()
    {
        CurrentMap.StartWave();
        Current_State = Game_State.Waving;
    }

    public void MapEnding()
    {
        CurrentMapIndex++;
        Player.Stop = true;
        Player.PlayerMovement.Stop = true;
        TVStarTransitionController.Instance.PlayPowerOffHold(false);
        //RewardUI.Open();
        Current_State = Game_State.MapEnd;
    }

    public void NextMap()
    {

    }

    public void Clear()
    {
        TVStarTransitionController.Instance.PlayPowerOffHold(false);
        Player.Stop = true;
        Current_State = Game_State.Clear;
    }

    public void FailDead()
    {
        TVStarTransitionController.Instance.PlayPowerOffHold(false);
        spawnManagers.Enemy.AllBack();
        Current_State = Game_State.Fail;
    }

    public UIWindow Reward;
    public UIWindow Result;

    public void OnPannel()
    {
        if(Current_State == Game_State.MapEnd)
        {
            Reward.Open();
        }
        else if(Current_State == Game_State.Fail || Current_State == Game_State.Clear)
        {
            Result.Open();
        }
    }
}

public enum Game_State
{
    GameInit,
    MapInit,
    Attack,
    Ready,
    Waving,
    MapEnd,
    Fail,
    Clear
}