using IWantGoHome.ScreenEffects;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-49)]
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public PlayerController Player;

    public PlayerInventoryManager Inventory;

    public Transform CurrentSpawnPoint;

    public event Action<PlayerController> EnemyInit;

    public SpawnManagers spawnManagers;

    public Game_State Current_State;

    public List<EnemyController> EnemyList;

    public EnemySpawnManager CurrentMap;

    [TextArea] public List<String> Texts;
    private void Awake()
    {
        Instance = this;

        Current_State = Game_State.Attack;

        Application.targetFrameRate = 120;
    }

    private void Start()
    {
        TVStarTransitionController.Instance.PlayPowerOnRelease();

        spawnManagers.Init();

        Player.PlayerInit(CurrentSpawnPoint);

        Player.PlayerInit();

        CurrentMap.Player = Player;
        CurrentMap.MapInit();

        Inventory.InitInventory();

        Inventory.GetIron(20);
        for(int i=0; i < 2; i++)
        {
            Inventory.GetCore(0);
        }

        EnemyInit?.Invoke(Player);

        foreach (EnemyController enemy in EnemyList)
        {
            enemy.OnDead += DeleteEnemy;
        }

        TutorialManager.Instance.NextInfo();
    }

    public void DeleteEnemy(EnemyController enemy)
    {
        EnemyList.Remove(enemy);
    }

    public bool IsAllKill()
    {
        return EnemyList.Count == 0;
    }

    public void Ready()
    {
        CurrentMap.ReadyWave(0);
        Player.UIController.Lock = false;
        Current_State = Game_State.Ready;
    }

    public void StartWave()
    {
        CurrentMap.StartWave();
        UIUpdateManager.Instance.ClearWaveSpawnWaypoints();
        Current_State = Game_State.Waving;
    }

    public void End()
    {
        if (spawnManagers.Enemy.EnemyCount == 0 && CurrentMap.IsSpawnEnd() && Current_State == Game_State.Waving)
        {
            TVStarTransitionController.Instance.PlayPowerOffHold();
        }
    }

    public void LobbyBack()
    {
        SceneManager.LoadScene(0);
    }

    public TextMeshProUGUI Info;

    int current = 0;

    public void NextInfo()
    {
        Info.text = Texts[current++];
    }
}
