using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CreftingUI : MonoBehaviour
{
    UIWindow window;

    public Image ItemIcon;
    public Animator ImageAnimator;
    public TextMeshProUGUI Count;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI NeedIngredient;
    public TextMeshProUGUI NeedCore;

    public MultiEnumCursor EnumCursor;

    public Crafter Crafter;

    FInteractionController FController;

    public bool stopped;

    private void Awake()
    {
        window = GetComponent<UIWindow>();
        window.Refreshed += ReFreshData;
        window.Closed += stop;
        window.Opened += open;

        EnumCursor = new MultiEnumCursor(
            typeof(Turret_Type),
            typeof(Item_Type)
        );

        Crafter = GetComponent<Crafter>();

        FController = UIUpdateManager.Instance.FController;
    }

    void open()
    {
        stopped = false;
    }

    void stop()
    {
        stopped = true;
        FController.SetActiveInteraction(false);
    }

    public void ReFreshData()
    {
        CraftingItemDataBase data = null;

        InventoryData count = new InventoryData();

        if (EnumCursor.Current is Turret_Type turretType)
        {
            data = GameDataManager.Instance.GetCraftingData(turretType);

            if (GameManager.Instance)
            {
                count = GameManager.Instance.Inventory.TurretInInventory[turretType];
            }
            else
            {
                count = TutorialManager.Instance.Inventory.TurretInInventory[turretType];
            }
        }
        else if(EnumCursor.Current is Item_Type itemType)
        {
            data = GameDataManager.Instance.GetCraftingData(itemType);

            count = new InventoryData();
        }

        //ui 변경하기
        Count.text = $"보유 수량 : {(count.InInvenCount + count.SpawnedCount).ToString("#,##0")} / {data.MaxCount.ToString("#,##0")}";
        Name.text = data.Name;
        Description.text = data.Description;
        NeedCore.text = $"필요 코어 : Lv.{data.Level+1}";
        NeedIngredient.text = $"소모 고철 : {data.needIron.ToString("#,##0")}";

        ItemIcon.sprite = data.Icon;

        if(GameManager.Instance)
        {
            if (GameManager.Instance.Current_State != Game_State.Ready)
            {
                FController.SetActiveInteraction(false);
                return;
            }
        }
        else
        {
            if (TutorialManager.Instance.Current_State != Game_State.Ready)
            {
                FController.SetActiveInteraction(false);
                return;
            }
        }


        FController.SetActiveInteraction(true);
    }

    float fPressTime;

    bool fpress;

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null || stopped)
        {
            return;
        }

        //if() 분기점 재료 부족 or 최대 수량 or 
        if (!Crafter.IsCanCraftirng(EnumCursor.Current))
        {
            if (Crafter.IsMax())
            {
                FController.SetText("재료 부족");
            }
            else
            {
                FController.SetText("최대 수량");
            }
        }
        else
        {
            FController.SetText("제작");
        }

        if (!keyboard.anyKey.isPressed && !fpress)
        {
            return;
        }

        if (keyboard.eKey.wasPressedThisFrame)
        {
            EnumCursor.MoveNext();
        }

        if (keyboard.qKey.wasPressedThisFrame)
        {
            EnumCursor.MovePrev();
        }

        if (keyboard.fKey.wasPressedThisFrame)
        {
            fpress = true;
        }

        if (keyboard.fKey.wasReleasedThisFrame)
        {
            fpress = false;
        }

        if (fpress&& Crafter.IsCanCraftirng(EnumCursor.Current))//&& Installer.IsInstall)
        {
            fPressTime += Time.deltaTime;
            FController.SetGauge(fPressTime / 0.75f);
            if (fPressTime >= 0.75f)
            {
                fpress = false;
                fPressTime = 0;

                if (Crafter.IsCanCraftirng(EnumCursor.Current))
                {
                    Crafter.Craft(EnumCursor.Current);
                }
            }
        }
        else
        {
            fpress = false;
            fPressTime = 0; FController.SetGauge(0);
        }
    }
}

public enum Item_Type
{
}

public class MultiEnumCursor
{
    private readonly List<Enum[]> enumValueGroups = new();
    private readonly List<Type> enumTypes = new();

    private int groupIndex;
    private int valueIndex;

    public MultiEnumCursor(params Type[] enumTypes)
    {

        foreach (Type enumType in enumTypes)
        {

            Array rawValues = Enum.GetValues(enumType);

            // 핵심: 값이 없는 enum은 커서 대상에서 제외
            if (rawValues.Length == 0)
            {
                continue;
            }

            Enum[] values = new Enum[rawValues.Length];

            for (int i = 0; i < rawValues.Length; i++)
            {
                values[i] = (Enum)rawValues.GetValue(i);
            }

            this.enumTypes.Add(enumType);
            enumValueGroups.Add(values);
        }

        groupIndex = 0;
        valueIndex = 0;
    }

    public Enum Current => enumValueGroups[groupIndex][valueIndex];

    public Type CurrentEnumType => enumTypes[groupIndex];

    public int GroupIndex => groupIndex;

    public int ValueIndex => valueIndex;

    public void MoveNext()
    {
        valueIndex++;

        if (valueIndex < enumValueGroups[groupIndex].Length)
        {
            return;
        }

        valueIndex = 0;
        groupIndex++;

        if (groupIndex >= enumValueGroups.Count)
        {
            groupIndex = 0;
        }
    }

    public void MovePrev()
    {
        valueIndex--;

        if (valueIndex >= 0)
        {
            return;
        }

        groupIndex--;

        if (groupIndex < 0)
        {
            groupIndex = enumValueGroups.Count - 1;
        }

        valueIndex = enumValueGroups[groupIndex].Length - 1;
    }

    public void Reset()
    {
        groupIndex = 0;
        valueIndex = 0;
    }
}