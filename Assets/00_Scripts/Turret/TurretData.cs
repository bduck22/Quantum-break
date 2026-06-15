using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/TurretData")]
public class TurretData : ScriptableObject
{
    public string TurretName;

    public Turret_Type Type;

    public float CoolTime;//발사 쿨타임

    private void OnValidate()
    {
        //if (CoolTime < (Data.Duration + 2))
        //{
        //    Debug.LogWarning(
        //        $"{nameof(CoolTime)} 값이 효과 지속시간보다 작아서 자동 보정했습니다. " +
        //        $"입력 값: {CoolTime}, 최소값: {(Data.Duration + 2)}",
        //        this
        //    );

        //    CoolTime = (Data.Duration + 2);
        //}
    }

    public DebuffData Data;

    public float Cognizance;
}

[Serializable]
public struct DebuffData
{
    public Buff_Type Type;

    public float Duration;

    public float Size;

    public float Power;
}
