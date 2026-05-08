using UnityEngine;

public class EnemyRouteController : MonoBehaviour
{
    public WaveSpawnData[] WaveSpawnDatas;

    public Route[] Routes;

    public int[] WayPointIndexs;
}

public class Route
{
    public Transform[] WayPoints;
}