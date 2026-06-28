using UnityEngine;

public class MainCoreController : MonoBehaviour
{
    public float TurretHp;
    public float MaxTurretHp;

    public void Coreinit(float MaxCoreHp)
    {
        MaxTurretHp = MaxCoreHp;
        UIUpdateManager.Instance.UpdateCoreHp(TurretHp, MaxTurretHp);
    }

    public void CoreActived()
    {
        TurretHp = MaxTurretHp;
        UIUpdateManager.Instance.UpdateCoreHp(TurretHp, MaxTurretHp);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.Current_State != Game_State.Waving) return;

        if(other.gameObject.layer == 10)
        {
            Hit();
        }
    }

    public void Hit()
    {
        TurretHp--;
        UIUpdateManager.Instance.UpdateCoreHp(TurretHp, MaxTurretHp);
        if (TurretHp <= 0)
        {
            GameManager.Instance.MapEnding();
        }
    }
}
