using UnityEngine;

public class CameraAnimationController : MonoBehaviour
{
    public Animator CamAnimator;

    bool walk;

    public void SetLeftWall()
    {
        if (walk)
        {
            walk = false;
        }
    }
    public void SetRightWall()
    {
        if (walk)
        {
            walk = false;
        }
    }

    public void SetWalk()
    {
        if (!walk)
        {
            walk = true;
            
        }
    }
}
