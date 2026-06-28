using IWantGoHome.ScreenEffects;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace IWantGoHome.ScreenEffects.Samples
{
    public sealed class TVStarTransitionDemoInput_NewInputSystem : MonoBehaviour
    {
        private void Update()
        {
#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            TVStarTransitionController controller = TVStarTransitionController.Instance;

            if (keyboard == null || controller == null)
            {
                return;
            }

            if (keyboard.f9Key.wasPressedThisFrame)
            {
                // 화면이 꺼지고, RGB 글리치 잔상 화면으로 고정됩니다.
                controller.PlayPowerOffHold(false);
            }
            else if (keyboard.f10Key.wasPressedThisFrame)
            {
                // F9 종료 상태에서 시작해서 암전 -> 발광/잔상 -> 정상 화면으로 돌아옵니다.
                controller.PlayPowerOnRelease();
            }
            else if (keyboard.f11Key.wasPressedThisFrame)
            {
                // 효과 즉시 종료.
                controller.HideImmediate();
            }
#else
            // This sample is for Unity's New Input System.
            // Enable/Install Input System, or call TVStarTransitionController methods from your own code.
#endif
        }
    }
}
