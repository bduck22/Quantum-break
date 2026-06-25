using UnityEngine;

[CreateAssetMenu(menuName = "Data/Map")]
public class MapData : ScriptableObject
{
    public string Name;
    public GameObject Map;
    public int Wave;
    public int Difficult;
}
