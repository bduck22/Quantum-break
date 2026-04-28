using UnityEngine;

public class CameraEffectController : MonoBehaviour
{
    public PlayerController PlayerController;

    public PlayerMovement PlayerMovement;

    CameraShake CameraShake;

    FOV FOV;

    public CameraAnimation CameraAnimation;

    public PostProcessingController PostProcessingController;

    public CameraHighlightLineDraw CameraHighlightLineDraw;

    public PlayerForwardEffectController PlayerForwardEffectController;

    private void Awake()
    {
        CameraShake = GetComponent<CameraShake>();
        FOV = GetComponent<FOV>();
        PostProcessingController = GetComponent<PostProcessingController>();
        CameraHighlightLineDraw = GetComponent<CameraHighlightLineDraw>();
        PlayerForwardEffectController = GetComponent<PlayerForwardEffectController>();
    }

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
        PlayerMovement.OnMoveStarted += CameraShake.Shake;
        PlayerMovement.OnMoveStarted += FOV.FOVUp;

        PlayerMovement.OnMoveStopped += CameraShake.StopShake;
        PlayerMovement.OnMoveStopped += FOV.BackFOV;

        PlayerMovement.OnDash += FOV.DashFOV;
        PlayerMovement.OnDash += FOV.BackFOV;
        PlayerMovement.OnDash += PostProcessingController.DashFilterOff;
        PlayerMovement.OnDash += CameraHighlightLineDraw.OnDash;
        PlayerMovement.OnDash += PlayerForwardEffectController.VfxPlay;

        PlayerMovement.OnStepped += CameraAnimation.BigShake;

        PlayerController.OnDashing += FOV.DashingFOV;
        PlayerController.OnDashing += PostProcessingController.DashFilterOn;

        PlayerController.OnLeftWall += CameraAnimation.SetLeftWall;
        PlayerController.OnRightWall += CameraAnimation.SetRightWall;
        PlayerController.OnWalk += CameraAnimation.SetWalk;
    }

    public void OffChain()
    {
        PlayerMovement.OnMoveStarted -= CameraShake.Shake;
        PlayerMovement.OnMoveStarted -= FOV.FOVUp;

        PlayerMovement.OnMoveStopped -= CameraShake.StopShake;
        PlayerMovement.OnMoveStopped -= FOV.BackFOV;

        PlayerMovement.OnDash -= FOV.DashFOV;
        PlayerMovement.OnDash -= FOV.BackFOV;
        PlayerMovement.OnDash -= PostProcessingController.DashFilterOff;
        PlayerMovement.OnDash -= CameraHighlightLineDraw.OnDash;
        PlayerMovement.OnDash -= PlayerForwardEffectController.VfxPlay;

        PlayerMovement.OnStepped -= CameraAnimation.BigShake;

        PlayerController.OnDashing -= FOV.DashingFOV;
        PlayerController.OnDashing -= PostProcessingController.DashFilterOn;

        PlayerController.OnLeftWall -= CameraAnimation.SetLeftWall;
        PlayerController.OnRightWall -= CameraAnimation.SetRightWall;
        PlayerController.OnWalk -= CameraAnimation.SetWalk;
    }
}
