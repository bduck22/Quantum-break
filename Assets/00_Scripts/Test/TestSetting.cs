using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TestSetting : MonoBehaviour
{
    public Image Setting;

    private void Update()
    {
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Setting.gameObject.SetActive(!Setting.gameObject.activeSelf);

            if(Setting.gameObject.activeSelf)
            {
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Time.timeScale = 1f; 
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
