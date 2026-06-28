using UnityEngine;

[CreateAssetMenu(menuName = "Card Effect/Heal Stamina")]
public class PlayerHealStamina : PlayerCardBase
{
    public override void Apply()
    {
        controller.StaminaHealSpeed += Data.Value;
    }
}
