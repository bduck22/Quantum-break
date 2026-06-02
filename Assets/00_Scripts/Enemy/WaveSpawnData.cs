using UnityEngine;

[CreateAssetMenu(menuName = "Data/WaveSpawnData")]
public class WaveSpawnData : ScriptableObject
{
    public Enemy_Type Type;
    public int SpawnCount;
    public float SpawnDelay;
//    public int SpawnPointIndex;
}

