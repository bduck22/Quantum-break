using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    public Animator CamAnimator;

    bool walk;

    public void SetLeftWall()
    {
        if (walk)
        {
            CamAnimator.SetBool("Walk", false);
            CamAnimator.SetTrigger("RightWall");
            walk = false;
        }
    }
    public void SetRightWall()
    {
        if (walk)
        {
            CamAnimator.SetBool("Walk", false);
            CamAnimator.SetTrigger("LeftWall");
            walk = false;
        }
    }

    public void SetWalk()
    {
        if (!walk)
        {
            CamAnimator.SetBool("Walk", true);
            walk = true;
        }
    }

    public void BigShake()
    {
        if (walk)
        {
            CamAnimator.SetTrigger("Big");
        }
    }
}
