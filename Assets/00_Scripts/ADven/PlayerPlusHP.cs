using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Effect/Plus Hp")]
public class PlayerPlusHP : PlayerCardBase
{
    public override void Apply()
    {
        controller.PlusHp+=(int)(Data.Value);
    }
}
