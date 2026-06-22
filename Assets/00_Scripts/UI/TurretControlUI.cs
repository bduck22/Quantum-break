using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TurretControlUI : MonoBehaviour
{
    UIWindow window;

    public Image TurretIcon;
    public Animator ImageAnimator;
    public TextMeshProUGUI Count;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;

    public Turret_Type CurrentType;

    public TurretInstaller Installer;

    private void Awake()
    {
        window = GetComponent<UIWindow>();
        window.Refreshed += ReFreshData;
        window.Closed += stop;
    }

    void stop()
    {
        Installer.CanInstall = false;
    }

    public void ReFreshData()
    {
        TurretData data = GameDataManager.Instance.GetData(CurrentType);

        //ui 반영하기

        InventoryData count = GameManager.Instance.Inventory.TurretInInventory[CurrentType];

        Installer.type = CurrentType;
        if (count.InInvenCount > 0)
        {
            Installer.CanInstall = true;
            Debug.Log("포탑이 있어!");
        }
        else
        {
            Installer.CanInstall = false;
            Debug.Log("포탑이 없어!");
        }
    }

    float fPressTime;

    bool fpress;

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
            NextPrevTurret(true);
        }

        if (keyboard.qKey.wasPressedThisFrame)
        {
            NextPrevTurret(false);
        }

        if (keyboard.fKey.wasPressedThisFrame)
        {
            fpress = true;
        }

        if (keyboard.fKey.wasReleasedThisFrame)
        {
            fpress = false;
        }

        if (fpress&& Installer.IsInstall)
        {
            fPressTime += Time.deltaTime;
            if(fPressTime >= 0.75f)
            {
                fpress = false;
                fPressTime = 0;

                if (Installer.Interaction())
                {
                    GameManager.Instance.Inventory.SpawnTurret(CurrentType);
                }
            }
        }
        else
        {
            fPressTime = 0;
        }
    }

    void NextPrevTurret(bool next)
    {
        int type = (int)CurrentType;
        if (next)
        {
            if(++type>= Enum.GetValues(typeof(Turret_Type)).Length)
            {
                type = 0;
            }
        }
        else
        {
            if (--type < 0)
            {
                type = Enum.GetValues(typeof(Turret_Type)).Length-1;
            }
        }

        CurrentType = (Turret_Type)type;
        ReFreshData();
    }
}
