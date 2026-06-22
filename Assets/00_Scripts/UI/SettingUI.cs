using UnityEngine;

public class SettingUI : MonoBehaviour
{
    UIWindow window;

    private void Awake()
    {
        window = GetComponent<UIWindow>();
        window.Opened += Open;
        window.Closed += Close;
    }

    void Open()
    {
        Time.timeScale = 0;
    }

    void Close()
    {
        Time.timeScale = 1;
    }
}
