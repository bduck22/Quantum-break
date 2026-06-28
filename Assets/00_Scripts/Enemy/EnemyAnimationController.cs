using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    public Animator animator;

    public void SetWalk()
    {
        animator.SetBool("IsAttack", false);
    }

    public void SetFind()
    {
        animator.SetBool("IsAttack", true);
    }

    public void SetAttack()
    {
        animator.SetTrigger("OnAttack");
    }

    public void SetDeath()
    {
        animator.SetTrigger("Death");
    }

    public void Initation()
    {
        animator.ResetTrigger("Death");
        animator.ResetTrigger("OnAttack");
        animator.SetBool("IsAttack", false);
    }
}
