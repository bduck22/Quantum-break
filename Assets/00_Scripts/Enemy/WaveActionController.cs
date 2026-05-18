using UnityEngine;

public class WaveActionController : MonoBehaviour
{
    public EnemySpawnManager SpawnManager;

    private void OnEnable()
    {
        for(int i=0; i< SpawnManager.Spawners.Length; i++)
        {
            SpawnManager.OnWaveStart += SpawnManager.Spawners[i].SpawnStart;
            SpawnManager.OnWaveStartInt += SpawnManager.Spawners[i].LoadRoute;
        }
    }
}
