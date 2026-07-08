using IWantGoHome.ScreenEffects;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public int type;

    private void Start()
    {
        Application.targetFrameRate = -1;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        TVStarTransitionController.Instance.PlayPowerOnRelease();
    }

    public void OnPlay(int type)
    {
        this.type = type;
        TVStarTransitionController.Instance.PlayPowerOffHold();
    }

    public void CursorOn()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void Run()
    {
        switch (type)
        {
            case 0:
                GameStart();
                break;
            case 1:
                Tutorial();
                break;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void GameStart()
    {
        SceneManager.LoadScene(1);
    }

    public void Tutorial()
    {
        SceneManager.LoadScene(2);
    }
}
