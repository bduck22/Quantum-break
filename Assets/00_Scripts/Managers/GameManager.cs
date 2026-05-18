using NUnit.Framework;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController Player;
    public Transform Core;

    public EnemySpawnManager CurrentMap;

    private void Start()
    {
        MapStart();
    }

    public void MapStart()
    {
        CurrentMap.Player = Player;
        CurrentMap.Core = Core;
        CurrentMap.MapStart();
    }
}
