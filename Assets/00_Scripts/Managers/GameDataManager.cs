using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;
using static CreftingUI;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [SerializedDictionary("Type", "Data")]
    [SerializeField] private SerializedDictionary<Turret_Type, TurretDataCollection> TurretDatas;

    private void Awake()
    {
        Instance = this;
        //if(Instance == null)
        //{
        //    Instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    public CraftingItemDataBase GetCraftingData(Item_Type type)
    {
        return null; // 아이템 추가시 수정
    }
    public CraftingItemDataBase GetCraftingData(Turret_Type type)
    {
        return TurretDatas[type].MakeData;
    }

    public TurretData GetData(Turret_Type type)
    {
        return TurretDatas[type].Data;
    }

    public bool IsData(Turret_Type type)
    {
        return TurretDatas.ContainsKey(type);
    }
}

[Serializable]
public class TurretDataCollection
{
    public TurretData Data;

    public TurretCrafingData MakeData;
}
