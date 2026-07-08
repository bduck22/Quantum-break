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

    public bool Lock;

    public bool CantOpen;

    public bool Opened;
    //public 

    public AudioClip OpenSound;
    public AudioClip CloseSound;

    AudioSource Audio;

    public SoundRandomPlayer ClosePlayer;
    public SoundRandomPlayer OpenPlayer;

    PlayerController controller;

    private void Awake()
    {
        Audio = GetComponent<AudioSource>();
        controller = transform.parent.GetComponent<PlayerController>();
    }

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
        if(Lock)
        {
            return;
        }

        if (controller.IsDead || controller.Stop)
        {
            if (Opened)
            {
                CloseUI();
            }
            return;
        }

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

        for (int i = 0; i < NumberKeys.Length-1; i++)
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
            CurrentOpened.Opened += OpenPlay;
            CurrentOpened.Closed += ClosePlay;
        }
        else
        {
            if(CurrentOpened != Slots[slot])
            {
                CurrentOpened.Opened -= OpenPlay;
                CurrentOpened.Closed -= ClosePlay;
                CurrentOpened.Close();
                CurrentOpened = Slots[slot];
                CurrentOpened.Opened += OpenPlay;
                CurrentOpened.Closed += ClosePlay;
            }
        }
        CurrentOpened.Trigger();
    }

    public void OpenPlay()
    {
        OpenPlayer.SoundPlay();
    }

    public void ClosePlay()
    {
        ClosePlayer.SoundPlay();
    }

    public void OpenUI()
    {
        UIUpdateManager.Instance.CountUpdate();
        OpenPlay();
        Opened = true;
        CurrentOpened = null;
        LeftCanMain.Open();
        RightCanMain.Open();
        OpenedUI?.Invoke();
    }

    public void CloseUI()
    {
        ClosePlay();
        Opened = false;
        LeftCanMain.Close();
        RightCanMain.Close();
        CurrentOpened?.Close();
        ClosedUI?.Invoke();
    }
}
