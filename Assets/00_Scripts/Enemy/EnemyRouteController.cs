using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRouteController : MonoBehaviour
{
    public WaveSpawnData[] WaveSpawnDatas;

    public List<Route> Routes;

    public int CurrentWave;

    public Color LineColor;

    [HideInInspector]
    public PlayerController player;
    [HideInInspector]
    public Transform Core;

    LineRenderer lineRenderer;

    [SerializeField] bool Spawning;

    public bool SpawnEnd;

    private void Start()
    {
        Routes = new List<Route>();

        lineRenderer = GetComponentInChildren<LineRenderer>();

        Init();
    }

    public void Init()
    {
        int WayCount = transform.childCount - 1;

        for (int i = 0; i < WayCount; i++)
        {
            int PointCount = transform.GetChild(i + 1).childCount;

            Routes.Add(new Route());
            Routes[i].WayPoints = new Transform[PointCount];
            for (int j = 0; j < PointCount; j++)
            {
                Routes[i].WayPoints[j] = transform.GetChild(i + 1).GetChild(j);
            }
        }
    }

    //float linefalsetime = 0;

    float spawningtime = 0;

    Transform[] WayPoints;

    int spawnCount;

    private void Update()
    {
        if (Spawning)
        {
            //linefalsetime += Time.deltaTime;
            if (lineRenderer.material.color.a < 0.2f)
            {
                Color color = LineColor;
                color.a = 0.2f;
                lineRenderer.material.color = color;
            }
            else if(lineRenderer.material.color.a != 0.2f)
            {
                lineRenderer.material.color -= Color.black * Time.deltaTime;
            }

            //if (linefalsetime > 2f)
            //{
            //    //lineRenderer.enabled = false;
            //    //lineRenderer.positionCount = 0;
            //    Spawning = false;
            //}

            if (spawningtime < WaveSpawnDatas[CurrentWave].SpawnDelay)
            {
                spawningtime += Time.deltaTime;
            }
            else
            {
                spawningtime = 0;
                Spawn();
            }

        }

        if (lineRenderer.enabled)
        {
            lineRenderer.material.mainTextureOffset -= new Vector2(3 * Time.deltaTime, 0);
            lineRenderer.material.mainTextureOffset = new Vector2(Mathf.Clamp(lineRenderer.material.mainTextureOffset.x, -1, 0), lineRenderer.material.mainTextureOffset.y);
            if (lineRenderer.material.mainTextureOffset.x <= -1)
            {
                lineRenderer.material.mainTextureOffset = new Vector2(0, lineRenderer.material.mainTextureOffset.y);
            }
        }
    }

    public void LoadRoute(int currentWave)
    {
        CurrentWave = currentWave;

        lineRenderer.enabled = true;
        lineRenderer.material.color = LineColor;

        Route route = Routes[WaveSpawnDatas[CurrentWave].SpawnPointIndex];

        lineRenderer.positionCount = route.WayPoints.Length;

        for (int i = 0; i < route.WayPoints.Length; i++)
        {
            Vector3 P = new Vector3(route.WayPoints[i].position.x, route.WayPoints[i].position.y + 0.5f, route.WayPoints[i].position.z);
            lineRenderer.SetPosition(i, P);
        }
        WayPoints = (Transform[])route.WayPoints.Clone();
        spawnCount = WaveSpawnDatas[currentWave].SpawnCount;

        UIUpdateManager.Instance.AddWaveSpawnWaypoint(
            route.WayPoints[0],
            WaveSpawnDatas[currentWave]
        );
        //SpawnStart();
    }

    public void SpawnStart()
    {
        Spawning = true;
        SpawnEnd = false;
        //linefalsetime = 0;
        spawningtime = WaveSpawnDatas[CurrentWave].SpawnDelay;
    }

    void Spawn()
    {
        if (spawnCount <= 0)
        {
            return;
        }

        spawnCount--;
        EnemyController enemy = SpawnManagers.Instance.Enemy.SpawnEnemy(Enemy_Type.Normal);
        enemy.Player = player;
        enemy.Core = Core;
        enemy.EnemyInit(WayPoints.Clone() as Transform[], Routes[WaveSpawnDatas[CurrentWave].SpawnPointIndex].WayPoints[0].position);

        enemy.gameObject.SetActive(true);

        if(spawnCount <= 0)
        {
            Spawning = false;
            SpawnEnd = true;
        }
    }
}

[Serializable]
public class Route
{
    public Transform[] WayPoints;
}