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

    FInteractionController FController;

    public bool stopped;
    private void Awake()
    {
        window = GetComponent<UIWindow>();
        window.Refreshed += ReFreshData;
        window.Closed += stop;
        window.Opened += open;

        FController = UIUpdateManager.Instance.FController;
    }
    void open()
    {
        stopped = false;
    }

    void stop()
    {
        stopped = true;
        Installer.CanInstall = false;
        FController.SetActiveInteraction(false);
    }

    public void ReFreshData()
    {

        TurretData data = GameDataManager.Instance.GetTurretData(CurrentType);

        CraftingItemDataBase data2 = GameDataManager.Instance.GetCraftingData(CurrentType);

        InventoryData count = GameManager.Instance.Inventory.TurretInInventory[CurrentType];

        //ui 반영하기
        TurretIcon.sprite = data2.Icon;
        Count.text = "보유 수량 : " + count.InInvenCount.ToString("#,##0");
        Name.text = data2.Name;
        Description.text = data2.Description;

        Installer.type = CurrentType;

        if (GameManager.Instance.Current_State != Game_State.Ready)
        {
            FController.SetActiveInteraction(false);
            Installer.CanInstall = false;
            return;
        }

        if (count.InInvenCount > 0)
        {
            Installer.CanInstall = true;
        }
        else
        {
            Installer.CanInstall = false;
        }
        FController.SetActiveInteraction(true);
    }

    float fPressTime;

    bool fpress;

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null||stopped)
        {
            return;
        }

        if (Installer.IsDelete())
        {
            FController.SetText("해체");
        }
        else
        {
            if (Installer.IsCanInstall())
            {
                FController.SetText("설치");
            }
            else
            {
                if (Installer.Checker.IsDuplip())
                {
                    FController.SetText("겹침");
                }
                else if (Installer.CanInstall)
                {
                    FController.SetText("바닥 필요");
                }
                else
                {
                    if(GameManager.Instance.Current_State != Game_State.Ready)
                    {
                        FController.SetText("전투 중");
                    }
                    else 
                    {
                        FController.SetText("수량 부족");
                    }    
                }
            }
        }

        if (!keyboard.anyKey.isPressed && !fpress)
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

        if (fpress&& Installer.IsCanInstall() && Installer.CanInstall)
        {
            fPressTime += Time.deltaTime;
            FController.SetGauge(fPressTime / 0.75f);
            if(fPressTime >= 0.75f)
            {
                fpress = false;
                fPressTime = 0;

                if (Installer.Interaction())
                {
                    GameManager.Instance.Inventory.SpawnTurret(CurrentType);
                    ReFreshData();
                }
            }
        }
        else
        {
            fpress = false;
            fPressTime = 0; FController.SetGauge(0);
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

        ImageAnimator.SetTrigger("Play");

        CurrentType = (Turret_Type)type;
        ReFreshData();
    }
}
