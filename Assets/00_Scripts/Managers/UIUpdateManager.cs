using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-10)]
public class UIUpdateManager : MonoBehaviour
{
    public static UIUpdateManager Instance;

    [SerializeField] private OverlayWaypointUI overlayWaypointUI;

    public TextMeshProUGUI IronCount;
    public TextMeshProUGUI[] coreCount = new TextMeshProUGUI[3];

    public Slider CoreHp1;
    public Slider CoreHp2;

    public Transform HpParent;

    public Slider StaminaSlider;

    public TextMeshProUGUI EnemyCount;
    private void Awake()
    {
        Instance = this;
    }

    public void CountUpdate()
    {
        PlayerInventoryManager Inventory;
        if (GameManager.Instance)
        {
            Inventory = GameManager.Instance.Inventory;
        }
        else
        {
            Inventory = TutorialManager.Instance.Inventory;
        }


            IronCount.text = Inventory.Iron.ToString("#,##0");
        for (int i = 0; i < coreCount.Length; i++)
        {
            coreCount[i].text = Inventory.CoreCounts[i].ToString("#,##0");
        }

    }
    public void AddWaveSpawnWaypoint(Transform targetPosition, WaveSpawnData waveSpawnData)
    {
        if (overlayWaypointUI == null)
        {
            return;
        }

        overlayWaypointUI.AddTarget(targetPosition, waveSpawnData);
    }

    public void ClearWaveSpawnWaypoints()
    {
        if (overlayWaypointUI == null)
        {
            return;
        }

        overlayWaypointUI.ClearTargets();
    }

    public UIController UIController;

    public FInteractionController FController;

    public CanvasGroup Canvas;

    public void OffCanvas()
    {
        Canvas.gameObject.SetActive(false);
    }

    public void OnCanvas()
    {
        Canvas.gameObject.SetActive(true);
    }

    private void Update()
    {
        //if (Canvas.gameObject.activeSelf&&Canvas.alpha !=1)
        //{
        //    Canvas.alpha += Time.deltaTime*1.5f;
        //    if(Canvas.alpha >= 1)
        //    {
        //        Canvas.alpha = 1;
        //    }
        //}
    }

    public GameObject PlusHP;

    public void UpdatePlayerHp(int Hp, int plus)
    {
        for (int i = 0; i < 3; i++)
        {
            if (Hp > i)
            {
                HpParent.GetChild(i).GetChild(0).GetComponent<Image>().color = Color.blue;
            }
            else
            {
                HpParent.GetChild(i).GetChild(0).GetComponent<Image>().color = Color.black * 0;
            }
        }

        int pluscount = plus - (HpParent.childCount - 3);

        //1 -> 0

        for (; pluscount!=0; )
        {
            if(pluscount > 0)
            {
                Instantiate(PlusHP, HpParent);
                pluscount--;
            }
            else if(pluscount < 0)
            {
                Destroy(HpParent.GetChild(3).gameObject);
                pluscount++;
            }
        }
    }

    public void UpdateEnemyCount()
    {
        if(GameManager.Instance.Current_State == Game_State.Waving)
        {
            EnemyCount.text = "남은 적 수 : " + GameManager.Instance.spawnManagers.Enemy.EnemyCount.ToString("#,##0");
        }
        else
        {
            EnemyCount.text = "남은 적 수 : " + GameManager.Instance.CurrentMap.DefaultEnemyCount.ToString("#,##0");
        }
    }

    public void UpdateCoreHp(float Hp, float max)
    {
        CoreHp1.value = Hp / max;
        CoreHp2.value = Hp / max;
    }

    public void UpdateStamina(float Stamina, float max)
    {
        StaminaSlider.value = Stamina / max;
    }
}