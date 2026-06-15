using UnityEngine;

public class TurretController : TurretBase
{
    float timer;

    public Transform Target;

    public float RotateSpeed;

    public Transform Model;

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
        Attack.Init(Data.Data);
        timer = Data.CoolTime;
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


        Quaternion targetrotation = Quaternion.Lerp(Model.rotation, Quaternion.LookRotation((Target.position + LookPivot) - Model.position), Time.deltaTime * RotateSpeed);
        Model.rotation = targetrotation;
    }

    void AttackAction()
    {
        if (Target == null) return;

        if(Data.CoolTime > timer)
        {
            if (Attack.IsCool())
            {
                timer += Time.deltaTime;
            }
        }
        else
        {
            timer = 0;
            Attack?.Attack(Target);
        }
    }
}
