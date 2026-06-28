using UnityEngine;
using UnityEngine.InputSystem.XR;

[CreateAssetMenu(menuName = "Card Effect/Max Stamina")]
public class PlayerMaxStamina : PlayerCardBase
{
    public override void Apply()
    {
        controller.MaxStamina += Data.Value;
    }
}
