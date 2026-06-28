using UnityEngine;

[CreateAssetMenu(menuName = "Data/Card")]
public class PlayerCardScriptableData : ScriptableObject
{
    public string Name;
    [TextArea] public string Description;

    public Sprite Icon;

    public float Value;

    public float ShowPer;

    public int MaxCount;
}
