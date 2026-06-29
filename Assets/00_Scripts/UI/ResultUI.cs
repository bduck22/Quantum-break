using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    UIWindow window;

    public TextMeshProUGUI Title;

    public TextMeshProUGUI Diffi;

    private void Awake()
    {
        window = GetComponent<UIWindow>();
        window.Opened += Open;
        window.Closed += Close;

        if(GameManager.Instance.Current_State == Game_State.Clear)
        {
            Title.text = "임무 성공";
        }
        else if (GameManager.Instance.Current_State == Game_State.Fail)
        {
            Title.text = "임무 실패";
        }

            string T = "";
        switch (GameDataManager.Instance.GameLevel)
        {
            case 0: T="훈련";break;
            case 1: T = "초급";break;
            case 2:T = "중급";break;
            case 3: T = "상급"; break;
        }

        Diffi.text = T;
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
