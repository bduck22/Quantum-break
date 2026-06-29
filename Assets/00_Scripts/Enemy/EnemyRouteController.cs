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

        if (!WaveSpawnDatas[CurrentWave])
        {
            lineRenderer.enabled = false;
            return;
        }

        if(WaveSpawnDatas[CurrentWave].SpawnCount <= 0)
        {
            lineRenderer.enabled = false;
            return;
        }

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
        if (!WaveSpawnDatas[CurrentWave])
        {
            lineRenderer.enabled = false;
            return;
        }

        if (WaveSpawnDatas[CurrentWave].SpawnCount <= 0)
        {
            lineRenderer.enabled = false;
            return;
        }

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


        Route route = Routes[WaveSpawnDatas[CurrentWave].SpawnPointIndex];
        spawnCount--;
        EnemyController enemy = SpawnManagers.Instance.Enemy.SpawnEnemy(Enemy_Type.Normal, Quaternion.Euler(0, GetYRotation(route.WayPoints[0].position, route.WayPoints[1].position), 0) );
        enemy.Player = player;
        enemy.Core = Core;
        enemy.EnemyInit(WayPoints.Clone() as Transform[], route.WayPoints[0].position);

        enemy.gameObject.SetActive(true);

        if(spawnCount <= 0)
        {
            Spawning = false;
            SpawnEnd = true;
        }
    }

    public float GetYRotation(Vector3 point0, Vector3 point1)
    {
        Vector3 direction = point1 - point0;

        // 높이 차이는 무시하고 XZ 평면 기준으로만 계산
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
        {
            return 0f;
        }

        float yAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        return yAngle;
    }
}

[Serializable]
public class Route
{
    public Transform[] WayPoints;
}