using UnityEngine;

public class ArmAnimationController : MonoBehaviour
{
    public Animator ArmAnimator;

    public bool walk;

    public bool IsCanAttack;

    [SerializeField] int CurrentAttackMotion;

    public float NextAttackMotionDelay;

    float AttackMotionTimer;

    public int MaxMotionCount;

    private void Start()
    {
        ArmAnimator = GetComponent<Animator>();
        IsCanAttack = true;
    }

    private void Update()
    {
        if(CurrentAttackMotion > 0)
        {
            if(CurrentAttackMotion >= MaxMotionCount)
            {
                AttackMotionTimer = 0;
                CurrentAttackMotion = 0;
            }

            if (IsCanAttack)
            {
                AttackMotionTimer -= Time.unscaledDeltaTime;
                if (AttackMotionTimer <= 0)
                {
                    AttackMotionTimer = 0;
                    CurrentAttackMotion = 0;
                }
            }
        }
    }

    public void SetLeftWall()
    {
        if (walk||ArmAnimator.GetBool("Air"))
        {
            ArmAnimator.SetBool("Walk", false);
            ArmAnimator.SetBool("Air", false);
            ArmAnimator.SetTrigger("RightWall");
            walk = false;
        }
    }
    public void SetRightWall()
    {
        if (walk|| ArmAnimator.GetBool("Air"))
        {
            ArmAnimator.SetBool("Walk", false);
            ArmAnimator.SetBool("Air", false);
            ArmAnimator.SetTrigger("LeftWall");
            walk = false;
        }
    }

    public void SetWalk()
    {
        if (!walk)
        {
            ArmAnimator.SetBool("Air", false);
            ArmAnimator.SetBool("Walk", true);
            walk = true;
        }
    }

    public void SetAir()
    {
        if (walk)
        {
            ArmAnimator.SetBool("Walk", false);
            ArmAnimator.SetBool("Air", true);
            walk = false;
        }
    }

    public void SetAttack()
    {
        if(CurrentAttackMotion == 0&& AttackMotionTimer==0)
        {
            if (Random.Range(0, 100) < 50)
            {
                CurrentAttackMotion = 2;
            }
        }
        AttackMotionTimer = NextAttackMotionDelay;
        ArmAnimator.SetTrigger($"Attack{CurrentAttackMotion+1}");
        CurrentAttackMotion++;
    }

    public void AttackStart()
    {
        IsCanAttack = false;
    }

    public void AttackEnd()
    {
        IsCanAttack = true;
    }

    public void SetAttackSpeed(float speed)
    {
        ArmAnimator.SetFloat("AttackSpeed", speed);
    }

    public void OnParried()
    {
        ArmAnimator.SetTrigger("Parring");
    }
}
