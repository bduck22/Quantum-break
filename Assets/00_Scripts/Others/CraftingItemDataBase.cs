using UnityEngine;

public class CraftingItemDataBase : ScriptableObject
{
    public int needIron;

    public int Level;
    public Crafting_Type itemType;

    public Sprite Icon;

    public string Name;

    [TextArea] public string Description;

    public int MaxCount;

}

public enum Crafting_Type
{
    Turret
}