using System;
using UnityEngine;

public class Crafter : MonoBehaviour
{
    private PlayerInventoryManager Inventory;

    private CraftingItemDataBase data;

    private void Awake()
    {
        Inventory = GameManager.Instance.Inventory;
    }

    public bool IsCanCraftirng(Enum Type)
    {
        data=null;

        if (Type is Turret_Type turrettype)
        {
            data = GameDataManager.Instance.GetCraftingData(turrettype);
        }
        else if(Type is Item_Type itemtype)
        {
            data = GameDataManager.Instance.GetCraftingData(itemtype);
        }

        if (data == null) return false;

        if (Inventory.HavingObject(data.Level, data.needIron))
        {
            return true;
        }

        return false;
    }

    public void Craft(Enum Type)
    {
        Inventory.UseCore(data.Level);

        Inventory.UseIron(data.needIron);

        Inventory.GetItem(Type);
    }
}
