using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    public event Action OpenedUI;
    public event Action ClosedUI;

    public UIWindow[] Slots;

    public UIWindow LeftCanMain;
    public UIWindow RightCanMain;

    public UIWindow CurrentOpened;

    public bool CantOpen;

    public bool Opened;
    //public 

    private static readonly Key[] NumberKeys =
    {
        Key.Digit1,
        Key.Digit2,
        Key.Digit3,
        Key.Digit4
    };

    private static readonly Key[] NumpadKeys =
    {
        Key.Numpad1,
        Key.Numpad2,
        Key.Numpad3,
        Key.Numpad4
    };

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

        if (keyboard.tabKey.wasPressedThisFrame)
        {
            if (Opened)
            {
                CloseUI();
            }
            else if(!CantOpen&&Time.timeScale == 1)
            {
                OpenUI();
            }
        }

        if (!Opened)
        {
            return;
        }

        for (int i = 0; i < NumberKeys.Length; i++)
        {
            if (keyboard[NumberKeys[i]].wasPressedThisFrame ||
                keyboard[NumpadKeys[i]].wasPressedThisFrame
                )
            {
                SelectSlot(i);
                return;
            }
        }
    }

    public void SelectSlot(int slot)
    {
        if(CurrentOpened == null)
        {
            CurrentOpened = Slots[slot];
        }
        else
        {
            if(CurrentOpened != Slots[slot])
            {
                CurrentOpened.Close();
                CurrentOpened = Slots[slot];
            }
        }
        CurrentOpened.Trigger();
    }

    public void OpenUI()
    {
        Opened = true;
        CurrentOpened = null;
        LeftCanMain.Open();
        RightCanMain.Open();
        OpenedUI?.Invoke();
    }

    public void CloseUI()
    {
        Opened = false;
        LeftCanMain.Close();
        RightCanMain.Close();
        CurrentOpened?.Close();
        ClosedUI?.Invoke();
    }
}
