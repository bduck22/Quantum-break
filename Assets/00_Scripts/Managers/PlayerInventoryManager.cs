using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    [SerializedDictionary("Type", "Count")]
    public SerializedDictionary<Turret_Type, InventoryData> TurretInInventory;

    public int Iron
    {
        get
        {
            return iron;
        }
    }
    [SerializeField]private int iron;

    public int[] CoreCounts;

    public void InitInventory()
    {
        for (int i = 0; i < TurretInInventory.Count; i++)
        {
            InventoryData data = TurretInInventory[(Turret_Type)i];

            data.InInvenCount = 0;
            data.SpawnedCount = 0;

            TurretInInventory[(Turret_Type)i] = data;
        }
        iron = 0;
        CoreCounts = new int[3] { 0, 0, 0 };
    }

    public bool IsHaveTurret(Turret_Type type)
    {
        if (TurretInInventory[type].InInvenCount > 0)
        {
            return true;
        }
        return false;
    }

    public void SpawnTurret(Turret_Type type)
    {
        InventoryData data = TurretInInventory[type];
        data.InInvenCount--;
        data.SpawnedCount++;
        TurretInInventory[type] = data;

        UIUpdateManager.Instance.UIController.CurrentOpened.Refresh();
    }

    public void ReturnTurret(Turret_Type type)
    {
        InventoryData data = TurretInInventory[type];
        data.InInvenCount++;
        data.SpawnedCount--;
        TurretInInventory[type] = data;

        UIUpdateManager.Instance.UIController.CurrentOpened.Refresh();
    }

    public void GetCore(int Level)
    {
        CoreCounts[Level]++;
    }

    public void GetIron(int Iron)
    {
        iron += Iron;
    }

    public bool HavingObject(int coreLevel, int ironcount)
    {
        if (IsUseIron(ironcount) &&IsUseCore(coreLevel))
        {
            return true;
        }
        return false;
    }

    bool IsUseIron(int count)
    {
        if (Iron >= count)
        {
            return true;
        }
        return false;
    }

    public void UseIron(int count)
    {
        iron -= count;
    }

    bool IsUseCore(int Level)
    {
        if(Level < 0 || Level >= CoreCounts.Length)
        {
            return true;
        }

        if (CoreCounts[Level] > 0)
        {
            return true;
        }
        return false;
    }

    public void UseCore(int Level)
    {
        CoreCounts[Level]--;
    }

    public void GetItem(Enum Type)
    {
        if (Type is Turret_Type turrettype)
        {
            InventoryData data = TurretInInventory[turrettype];
            data.InInvenCount++;
            TurretInInventory[turrettype] = data;
        }
        else if (Type is Item_Type itemtype)
        {
           //아이템 추가 시 작성
        }

        UIUpdateManager.Instance.UIController.CurrentOpened.Refresh();
    }
}

[Serializable]
public struct InventoryData
{
    public int InInvenCount;
    public int SpawnedCount;
}

