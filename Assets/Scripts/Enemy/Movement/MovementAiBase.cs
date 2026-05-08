using UnityEngine;
using System.Collections.Generic;
using System;

public interface MovementAI
{
    public void OnStart();
    public void OnMove();
    public void OnStop();
}

public abstract class MovementAIBase : MonoBehaviour, MovementAI
{
    public List<Transform> WayPoints;

    public int CurrentWayPIndex;

    public event Action OnWayPoint;

    public bool IsMoving;
    public virtual void OnStart()
    {
        IsMoving = true;
    }
    public virtual void OnMove()
    {

    }
    public virtual void OnStop()
    {
        IsMoving = false;
    }
    public abstract void Init();
}
