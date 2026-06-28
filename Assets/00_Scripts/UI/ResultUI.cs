using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
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
        Cursor.lockState = CursorLockMode.None;
    }

    void Close()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.GoMain();
    }
}
