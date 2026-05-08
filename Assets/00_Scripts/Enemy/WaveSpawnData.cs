using UnityEngine;

[CreateAssetMenu(menuName = "Data/WaveSpawnData")]
public class WaveSpawnData : ScriptableObject
{
    public GameObject EnemyPrefab;
    public int SpawnCount;
    public float SpawnDelay;
}

