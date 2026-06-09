using UnityEngine;

public class TurretController : TurretBase
{
    float timer;

    public Transform Target;

    public float RotateSpeed;

    [SerializeField]
    private Vector3 LookPivot;

    [Header("스크립트")]
    public TurretTargetFinder TargetFinder;
    public TurretAttackBase Attack;

    [Header("인식범위표시")]
    public SphereCollider collider;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        timer = 0;
        CognizanceRefresh();
    }

    public void CognizanceRefresh()
    {
        collider.radius = Data.Cognizance;
    }

    private void Update()
    {
        TargetRotation();
    
        AttackAction();
    }

    void TargetRotation()
    {
        Target = TargetFinder?.GetTarget();

        if (Target == null)
        {
            return;
        }


        Quaternion targetrotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((Target.position + LookPivot) - transform.position), Time.deltaTime * RotateSpeed);
        transform.rotation = targetrotation;
    }

    void AttackAction()
    {
        if(Data.CoolTime > timer)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            Attack?.Attack();
        }
    }
}
