using System;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public event Action OnAttack;

    public void Attack()
    {
        OnAttack?.Invoke();
    }
}
