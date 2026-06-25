using IWantGoHome.ScreenEffects;
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

    public ScreenGhostEffect ScreenGhostEffect;

    public PlayerHitEffect PlayerHitEffect;

    public PlayerParringEffect PlayerParringEffect;

    public SoundRandomPlayer DashPlayer;

    public SoundRandomPlayer DashingPlayer;

    public SlowMotionAudioFilter AudioFilter;

    public PlayerHitAfterimageController PlayerHitAfterimageController;

    private void Awake()
    {
        CameraShake = GetComponent<CameraShake>();
        FOV = GetComponent<FOV>();
        PostProcessingController = GetComponent<PostProcessingController>();
        CameraHighlightLineDraw = GetComponent<CameraHighlightLineDraw>();
        PlayerForwardEffectController = GetComponent<PlayerForwardEffectController>();
        ScreenGhostEffect = GetComponent<ScreenGhostEffect>();  
        PlayerHitEffect = GetComponent<PlayerHitEffect>();
        PlayerParringEffect = GetComponent<PlayerParringEffect>();
        DashingPlayer = GetComponent<SoundRandomPlayer>();
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
        PlayerMovement.OnDash += ScreenGhostEffect.PlayEffect;
        PlayerMovement.OnDash += DashPlayer.SoundPlay;
        PlayerMovement.OnDash += DashingPlayer.Stop;
        PlayerMovement.OnDash += AudioFilter.ExitSlowMotion;

        PlayerMovement.OnBigStepped += CameraAnimation.BigShake;

        PlayerController.OnDashing += FOV.DashingFOV;
        PlayerController.OnDashing += PostProcessingController.DashFilterOn;
        PlayerController.OnDashing += DashingPlayer.SoundPlay;
        PlayerController.OnDashing += AudioFilter.EnterSlowMotion;

        PlayerController.OnLeftWall += CameraAnimation.SetLeftWall;
        PlayerController.OnRightWall += CameraAnimation.SetRightWall;
        PlayerController.OnWalk += CameraAnimation.SetWalk;

        PlayerController.OnAttack += CameraAnimation.BigShake;

        PlayerController.OnHit += PlayerHitEffect.OnHit;
        PlayerController.OnHit += PostProcessingController.HitFilterOn;
        PlayerController.OnHit += FOV.HitedFOV;
        PlayerController.OnHit += CameraAnimation.BigShake;
        PlayerController.OnHit += PlayerHitAfterimageController.PlayHit;

        PlayerController.EndHitInvincibility += PlayerHitEffect.EndHit;
        PlayerController.EndHitInvincibility += PostProcessingController.HitFilterOff;
        PlayerController.EndHitInvincibility += FOV.HitBackFOV;

        PlayerController.OnParried += PlayerParringEffect.OnParring;
        PlayerController.OnParried += CameraAnimation.ParringShake;
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
        PlayerMovement.OnDash -= ScreenGhostEffect.PlayEffect;
        PlayerMovement.OnDash -= DashPlayer.SoundPlay;
        PlayerMovement.OnDash -= DashingPlayer.Stop;
        PlayerMovement.OnDash -= AudioFilter.ExitSlowMotion;

        PlayerMovement.OnBigStepped -= CameraAnimation.BigShake;

        PlayerController.OnDashing -= FOV.DashingFOV;
        PlayerController.OnDashing -= PostProcessingController.DashFilterOn;
        PlayerController.OnDashing -= DashingPlayer.SoundPlay;
        PlayerController.OnDashing -= AudioFilter.EnterSlowMotion;

        PlayerController.OnLeftWall -= CameraAnimation.SetLeftWall;
        PlayerController.OnRightWall -= CameraAnimation.SetRightWall;
        PlayerController.OnWalk -= CameraAnimation.SetWalk;

        PlayerController.OnAttack -= CameraAnimation.BigShake;

        PlayerController.OnHit -= PlayerHitEffect.OnHit;
        PlayerController.OnHit -= PostProcessingController.HitFilterOn;
        PlayerController.OnHit -= FOV.HitedFOV;
        PlayerController.OnHit -= CameraAnimation.BigShake;
        PlayerController.OnHit -= PlayerHitAfterimageController.PlayHit;

        PlayerController.EndHitInvincibility -= PlayerHitEffect.EndHit;
        PlayerController.EndHitInvincibility -= PostProcessingController.HitFilterOff;
        PlayerController.EndHitInvincibility -= FOV.HitBackFOV;

        PlayerController.OnParried -= PlayerParringEffect.OnParring;
        PlayerController.OnParried -= CameraAnimation.ParringShake;
    }
}
