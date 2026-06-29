using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    UIWindow window;

    public Image ItemIcon;
    public TextMeshProUGUI Count;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;

    public Player_Card_Type CurrentType;

    private void Awake()
    {
        window = GetComponent<UIWindow>();
        window.Refreshed += ReFreshData;
    }

    public void ReFreshData()
    {
        
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        if (!keyboard.anyKey.isPressed)
        {
            return;
        }

        if (keyboard.eKey.wasPressedThisFrame)
        {
            Next();
        }

        if (keyboard.qKey.wasPressedThisFrame)
        {
            Prev();
        }
    }

    public void Next()
    {
        int num = (int)CurrentType;
        if(++num >= sizeof(Player_Card_Type))
        {
            num = 0;
        }

        CurrentType = (Player_Card_Type)num;

        ReFreshData();
    }

    public void Prev()
    {
        int num = (int)CurrentType;
        if (--num < 0)
        {
            num = sizeof(Player_Card_Type)-1;
        }

        CurrentType = (Player_Card_Type)num;

        ReFreshData();
    }
}
