using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    public PlayerAttack PlayerAttack;

    public CameraAnimation CameraAnimation;

    private void OnEnable()
    {
        PlayerAttack.OnAttack += CameraAnimation.BigShake;
    }

    private void OnDisable()
    {
        PlayerAttack.OnAttack -= CameraAnimation.BigShake;
    }
}
