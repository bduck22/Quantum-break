using System;
using UnityEngine;

public class Crafter : MonoBehaviour
{
    private PlayerInventoryManager Inventory;

    private CraftingItemDataBase data;

    private InventoryData InvenData;

    private void Awake()
    {
        if (GameManager.Instance)
        {
            Inventory = GameManager.Instance.Inventory;
        }
        else if (TutorialManager.Instance)
        {
            Inventory = TutorialManager.Instance.Inventory;
        }
    }

    public bool IsCanCraftirng(Enum Type)
    {
        data=null;

        InvenData = new InventoryData();

        if (Type is Turret_Type turrettype)
        {
            data = GameDataManager.Instance.GetCraftingData(turrettype);
            InvenData = Inventory.TurretInInventory[turrettype];
        }
        else if(Type is Item_Type itemtype)
        {
            data = GameDataManager.Instance.GetCraftingData(itemtype);
        }

        if (data == null) return false;

        if (Inventory.HavingObject(data.Level, data.needIron)&&(InvenData.InInvenCount + InvenData.SpawnedCount) < data.MaxCount)
        {
            return true;
        }

        return false;
    }

    public bool IsMax()
    {
        return (InvenData.InInvenCount + InvenData.SpawnedCount) < data.MaxCount;
    }

    public void Craft(Enum Type)
    {
        Inventory.UseCore(data.Level);

        Inventory.UseIron(data.needIron);

        Inventory.GetItem(Type);

        UIUpdateManager.Instance.CountUpdate();
    }
}
