using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

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
}
