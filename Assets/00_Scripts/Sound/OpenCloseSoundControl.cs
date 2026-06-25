using UnityEngine;

[RequireComponent (typeof(UIWindow), typeof(SoundRandomPlayer), typeof(SoundRandomPlayer))]
public class OpenCloseSoundControl : MonoBehaviour
{
    public SoundRandomPlayer Open;

    public SoundRandomPlayer Close;

    public UIWindow uiwindow;

    private void Awake()
    {
        if (!uiwindow)
        {
            uiwindow = GetComponent<UIWindow>();    
        }

        uiwindow.Opened += Open.SoundPlay;
        uiwindow.Closed += Close.SoundPlay;
    }
}
