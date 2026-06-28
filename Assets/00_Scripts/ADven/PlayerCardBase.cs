using System;
using UnityEngine;

public abstract class PlayerCardBase : ScriptableObject
{
    public PlayerCardScriptableData Data;

    public Player_Card_Type Type;

    public PlayerController controller;

    public virtual void Init(PlayerController player)
    {
        controller = player;
    }

    public abstract void Apply();
}

public enum Player_Card_Type
{
    PlusHP,
    Stamina,
    MaxStamina
}
