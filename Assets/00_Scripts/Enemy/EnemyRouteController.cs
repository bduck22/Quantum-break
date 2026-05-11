using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRouteController : MonoBehaviour
{
    public WaveSpawnData[] WaveSpawnDatas;

    public List<Route> Routes;

    LineRenderer lineRenderer;

    private void Start()
    {
        Routes = new List<Route>();

        lineRenderer = GetComponentInChildren<LineRenderer>();

        Init();
    }

    public void Init()
    {
        int WayCount = transform.childCount-1;

        for (int i = 0; i < WayCount; i++)
        {
            int PointCount = transform.GetChild(i+1).childCount;

            Routes.Add(new Route());
            Routes[i].WayPoints = new Transform[PointCount];
            for (int j=0;j< PointCount; j++)
            {
                Routes[i].WayPoints[j] = transform.GetChild(i+1).GetChild(j);
            }
        }

        LoadRoute(0);
    }

    void LoadRoute(int currentWave)
    {
        lineRenderer.positionCount = Routes[currentWave].WayPoints.Length;

        for(int i = 0; i< Routes[currentWave].WayPoints.Length; i++)
        {
            Vector3 P = new Vector3(Routes[currentWave].WayPoints[i].position.x, Routes[currentWave].WayPoints[i].position.y + 0.5f, Routes[currentWave].WayPoints[i].position.z);
            lineRenderer.SetPosition(i, P);
        }
    }

    public void Spawn(int index)
    {
        //Instantiate
    }
}

[Serializable]
public class Route
{
    public Transform[] WayPoints;
}