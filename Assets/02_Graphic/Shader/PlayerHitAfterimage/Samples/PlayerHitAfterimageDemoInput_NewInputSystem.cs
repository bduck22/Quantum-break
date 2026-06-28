using IWantGoHome.ScreenEffects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IWantGoHome.ScreenEffects.Samples
{
    public sealed class PlayerHitAfterimageDemoInput_NewInputSystem : MonoBehaviour
    {
        [SerializeField] private PlayerHitAfterimageController controller;
        [SerializeField] private Key testHitKey = Key.F8;
        [SerializeField] private Key hideKey = Key.F11;

        private void Reset()
        {
            controller = FindFirstObjectByType<PlayerHitAfterimageController>();
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;

            PlayerHitAfterimageController target = controller != null ? controller : PlayerHitAfterimageController.Instance;
            if (target == null) return;

            if (keyboard[testHitKey].wasPressedThisFrame)
            {
                target.PlayHit();
            }

            if (keyboard[hideKey].wasPressedThisFrame)
            {
                target.HideImmediate();
            }
        }
    }
}
