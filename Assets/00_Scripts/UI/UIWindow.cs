using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIWindow : MonoBehaviour
{
    public Animator animator;

    public event Action Opened;
    public event Action Closed;
    public event Action Refreshed;

    

    public void Close()
    {
        animator.SetTrigger("Close");
        Closed?.Invoke();
    }

    public void Open()
    {
        gameObject.SetActive(true);
        Opened?.Invoke();
        Refreshed?.Invoke();
    }

    public void Refresh()
    {
        Refreshed?.Invoke();
    }

    public void Trigger()
    {
        if (gameObject.activeSelf)
        {
            Close();
        }
        else
        {
            Open();
        }
    }
}
