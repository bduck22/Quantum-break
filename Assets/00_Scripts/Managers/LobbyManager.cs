using IWantGoHome.ScreenEffects;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    private void Start()
    {
        TVStarTransitionController.Instance.PlayPowerOnRelease();
    }
}
