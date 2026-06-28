using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    [SerializedDictionary("Type", "Count")]
    public SerializedDictionary<Turret_Type, InventoryData> TurretInInventory;

    [SerializedDictionary("Type", "Level")]
    public SerializedDictionary<Player_Card_Type, PlayerCardData> PlayerCardInInventory;

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

        for (int i = 0; i < PlayerCardInInventory.Count; i++)
        {
            Player_Card_Type type = (Player_Card_Type)i;

            PlayerCardData data = PlayerCardInInventory[type];
            data.HavingCount = 0;

            data.MaxCount = GameDataManager.Instance.GetCardData(type).Data.MaxCount;

            PlayerCardInInventory[type] = data;
        }
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

    public bool IsCanGetCard(Player_Card_Type type)
    {
        PlayerCardData data = PlayerCardInInventory[type];

        if(data.MaxCount == 0)
        {
            return true;
        }

        if (data.HavingCount < data.MaxCount)
        {
            return true;
        }
        return false;
    }

    public void GetCard(Player_Card_Type type)
    {
        PlayerCardData data = PlayerCardInInventory[type];
        if(data.HavingCount < data.MaxCount)
        {
            data.HavingCount++;
        }
        PlayerCardInInventory[type] = data;
    }
}

[Serializable]
public struct InventoryData
{
    public int InInvenCount;
    public int SpawnedCount;
}

[Serializable]
public struct PlayerCardData
{
    public int HavingCount;
    public int MaxCount;
}

