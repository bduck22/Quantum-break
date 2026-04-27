using UnityEngine;

public class ArmAnimationController : MonoBehaviour
{
    public Animator ArmAnimator;

    public bool walk;

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
}
