using System;
using System.Collections.Generic;
using System.Linq;
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

        lineRenderer.positionCount = Routes[currentWave].WayPoints.Length;

        for (int i = 0; i < Routes[currentWave].WayPoints.Length; i++)
        {
            Vector3 P = new Vector3(Routes[currentWave].WayPoints[i].position.x, Routes[currentWave].WayPoints[i].position.y + 0.5f, Routes[currentWave].WayPoints[i].position.z);
            lineRenderer.SetPosition(i, P);
        }
        WayPoints = (Transform[])Routes[CurrentWave].WayPoints.Clone();
        spawnCount = WaveSpawnDatas[currentWave].SpawnCount;
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
            Spawning = false;
            SpawnEnd = true;
            return;
        }

        spawnCount--;
        EnemyController enemy = SpawnManagers.Instance.Enemy.SpawnEnemy(Enemy_Type.Normal);
        enemy.Player = player;
        enemy.Core = Core;
        enemy.EnemyInit(WayPoints.Clone() as Transform[], Routes[CurrentWave].WayPoints[0].position);

        enemy.gameObject.SetActive(true);
    }
}

[Serializable]
public class Route
{
    public Transform[] WayPoints;
}