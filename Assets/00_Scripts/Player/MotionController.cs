using UnityEngine;

public class MotionController : MonoBehaviour
{
    public PlayerController PlayerController;

    public ArmAnimationController ArmAnimation;

    private void OnEnable()
    {
        OnChain();
    }

    void OnDisable()
    {
        OffChain();
    }

    public void OnChain()
    {
        PlayerController.OnLeftWall += ArmAnimation.SetLeftWall;
        PlayerController.OnRightWall += ArmAnimation.SetRightWall;
        PlayerController.OnWalk += ArmAnimation.SetWalk;
        PlayerController.OnAir += ArmAnimation.SetAir;

        PlayerController.OnAttack += ArmAnimation.SetAttack;

        PlayerController.OnParried += ArmAnimation.OnParried;
    }

    public void OffChain()
    {

        PlayerController.OnLeftWall -= ArmAnimation.SetLeftWall;
        PlayerController.OnRightWall -= ArmAnimation.SetRightWall;
        PlayerController.OnWalk -= ArmAnimation.SetWalk;
        PlayerController.OnAir -= ArmAnimation.SetAir;

        PlayerController.OnAttack -= ArmAnimation.SetAttack;

        PlayerController.OnParried += ArmAnimation.OnParried;
    }
}
