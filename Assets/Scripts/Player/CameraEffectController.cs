using UnityEngine;

public class CameraEffectController : MonoBehaviour
{
    public PlayerMovement PlayerMovement;

    CameraShake CameraShake;

    FOV FOV;

    private void Awake()
    {
        CameraShake = GetComponent<CameraShake>();
        FOV = GetComponent<FOV>();
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
    }

    public void OffChain()
    {
        PlayerMovement.OnMoveStarted -= CameraShake.Shake;
        PlayerMovement.OnMoveStarted -= FOV.FOVUp;

        PlayerMovement.OnMoveStopped -= CameraShake.StopShake;
        PlayerMovement.OnMoveStopped -= FOV.BackFOV;
    }
}
