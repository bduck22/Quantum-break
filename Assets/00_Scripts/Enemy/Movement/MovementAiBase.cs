using UnityEngine;
using System.Collections.Generic;
using System;

public interface MovementAI
{
    public void OnStart(EnemyInfomation Info);
    public void OnMove();
    public void OnStop();
}

public abstract class MovementAIBase : MonoBehaviour, MovementAI
{
    public Transform[] WayPoints;

    public int CurrentWayPIndex;

    public event Action OnWayPoint;

    public bool FinalArrived;

    public bool IsMoving;
    public abstract void OnStart(EnemyInfomation Info);
    public abstract void OnMove();
    public abstract void OnStop();
    public virtual void Init(Transform[] wayPoints)
    {
        WayPoints = (Transform[])wayPoints.Clone();
        CurrentWayPIndex = 0;
    }

    protected void InvokeOnWayPoint()
    {
        OnWayPoint?.Invoke();
    }
}
