using UnityEngine;

public class SlowDebuffTurret : TurretDebuffBase
{
    ParticleController attackOb;
    private void Awake()
    {
        attackOb = AttackObject.GetComponent<ParticleController>();
    }

    readonly private Vector3 RayPivot = new Vector3(0,1,0);
    [SerializeField] private LayerMask groundLayer;

    public override void Attack(Transform Target)
    {
        ShootParticle.Play();

        RaycastHit hit;

        if (Physics.Raycast(Target.position + RayPivot, Vector3.down,out hit ,  3, groundLayer))
        {
            attackOb.InitWithTimer(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), DeBuff.Data.Duration, DeBuff.Data.Size);
            AttackObject.gameObject.SetActive(true);
        }
    }
}
