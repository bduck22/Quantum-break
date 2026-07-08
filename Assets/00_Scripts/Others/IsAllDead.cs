using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class IsAllDead : MonoBehaviour
{
    public List<EnemyController> enemies;

    HologramWallDisappearV5 hologramWall;

    private void Awake()
    {
        hologramWall = GetComponent<HologramWallDisappearV5>();
    }

    private void Start()
    {
        foreach (EnemyController enemy in enemies)
        {
            enemy.OnDead += DeleteEnemy;
        }
    }

    public void DeleteEnemy(EnemyController enemy)
    {
        enemies.Remove(enemy);
    }

    private void Update()
    {
        if(enemies.Count == 0)
        {
            hologramWall.IsOpen = true;
            this.enabled = false;
        }
    }
}
