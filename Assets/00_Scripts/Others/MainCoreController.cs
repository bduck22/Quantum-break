using UnityEngine;

public class MainCoreController : MonoBehaviour
{
    public float TurretHp;
    public float MaxTurretHp;

    public void Coreinit()
    {
        //TurretHp = MaxTurretHp;
    }

    public void CoreActived()
    {
        TurretHp = MaxTurretHp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 10)
        {
            TurretHp--;
        }
    }
}
