using System;
using UnityEngine;

public class BuffController : MonoBehaviour
{
    public event Action OnBuffStart;

    private void Start()
    {
        OnBuffStart?.Invoke();
        OnBuffStart = null;
    }
}
