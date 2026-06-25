using UnityEngine;

public class CraftingItemDataBase : ScriptableObject
{
    public int needIron;

    public int Level;
    public Crafting_Type itemType;


    public int MaxCount;

}

public enum Crafting_Type
{
    Turret
}