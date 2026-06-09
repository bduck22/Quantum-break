using UnityEngine;

[CreateAssetMenu(menuName = "Data/TurretData")]
public class TurretData : ScriptableObject
{
    public string TurretName;

    public Turret_Type Type;

    public float CoolTime;//발사 쿨타임

    public float Duration;//효과 지속시간

    public float Cognizance;
}
