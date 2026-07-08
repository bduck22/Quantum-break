using IWantGoHome.ScreenEffects;
using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Game_State Current_State;
    private Game_State Previous_State;

    public int WaveCount;
    public int CurrentWaveIndex;

    public bool Cleared;
    public bool Cardget;

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

    public float MaxCoreHp;

    public MapData Map1;
    public MapData Map2;

    public MapData SelectedMap;

    public Transform MapP;

    public event Action<PlayerController> OnDefaultEnemyInit;

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
        Cardget = false;
        spawnManagers.Init();
        Inventory.InitInventory();
        Current_State = Game_State.MapInit;
        MaxMapIndex = GameDataManager.Instance.GetRandomRoomCount();
        CurrentMapIndex = 0;
        Application.targetFrameRate = 120;
        SelectedMap = GameDataManager.Instance.GetMap(CurrentMapIndex);
        CurrentWaveIndex = 0;
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
        Cleared = false;
        Cardget = false;
        TVStarTransitionController.Instance.PlayPowerOnRelease();
        if (CurrentMap)
        {
            Destroy(CurrentMap.gameObject);
        }

        GameDataManager.Instance.SetMapUsed(SelectedMap);

        CurrentWaveIndex = 0;

        WaveCount = SelectedMap.Wave;

        CurrentMap = Instantiate(SelectedMap.Map, MapP).GetComponent<EnemySpawnManager>();

        CurrentMap.Player = Player;

        Core = CurrentMap.Core.GetComponent<MainCoreController>();
        Core.Coreinit(MaxCoreHp);

        CurrentMap.MapInit();
        Player.PlayerInit(CurrentMap.PlayerSpawnPoint);
        OnDefaultEnemyInit?.Invoke(Player);
        OnDefaultEnemyInit = null;

        Current_State = Game_State.Attack;
    }

    public void PlayerInit()
    {
        Player.PlayerInit();
    }

    public void ReadyWave()
    {
        Current_State = Game_State.Ready;
        CurrentWaveIndex++;
        CurrentMap.ReadyWave(CurrentWaveIndex - 1);
    }

    public void CheckWaveEnd()
    {
        if (spawnManagers.Enemy.EnemyCount==0&&CurrentMap.IsSpawnEnd()&&Current_State == Game_State.Waving)
        {
            if(WaveCount > CurrentWaveIndex)
            {
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
        UIUpdateManager.Instance.ClearWaveSpawnWaypoints();
        Current_State = Game_State.Waving;
    }

    public void MapEnding()
    {
        if (CurrentMapIndex >= MaxMapIndex)
        {
            Clear();
            return;
        }

        if (!Cleared) Cardget = true;
        Current_State = Game_State.MapEnd;
        CurrentMapIndex++;
        Player.Stop = true;
        Player.PlayerMovement.Stop = true;
        spawnManagers.Enemy.AllBack();
        spawnManagers.Turret.AllBack();
        NextMap();
        TVStarTransitionController.Instance.PlayPowerOffHold(false);
        //RewardUI.Open();
    }

    public void NextMap()
    {
        Map1 = GameDataManager.Instance.GetMap(CurrentMapIndex);
        Map2 = GameDataManager.Instance.GetMap(CurrentMapIndex);
    }

    public void Clear()
    {
        Current_State = Game_State.Clear;
        Player.Stop = true;
        Player.PlayerMovement.Stop = true;
        TVStarTransitionController.Instance.PlayPowerOffHold(false);
        spawnManagers.Enemy.AllBack();
        spawnManagers.Turret.AllBack();
        Player.Stop = true;
    }

    public void FailDead()
    {
        Current_State = Game_State.Fail;
        Player.Stop = true;
        Player.PlayerMovement.Stop = true;
        TVStarTransitionController.Instance.PlayPowerOffHold(false);
        spawnManagers.Enemy.AllBack();
        spawnManagers.Turret.AllBack();
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

    public void GoMain()
    {
        SceneManager.LoadScene(0);
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