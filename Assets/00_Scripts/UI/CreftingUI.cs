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

    public MultiEnumCursor EnumCursor;

    public Crafter Crafter;

    private void Awake()
    {
        window = GetComponent<UIWindow>();
        window.Refreshed += ReFreshData;

        EnumCursor = new MultiEnumCursor(
            typeof(Turret_Type),
            typeof(Item_Type)
        );

        Crafter = GetComponent<Crafter>();  
    }

    public void ReFreshData()
    {
        CraftingItemDataBase data;

        InventoryData count;

        if(EnumCursor.Current is Turret_Type turretType)
        {
            data = GameDataManager.Instance.GetCraftingData(turretType);

            count = GameManager.Instance.Inventory.TurretInInventory[turretType];
        }
        else if(EnumCursor.Current is Item_Type itemType)
        {
            data = GameDataManager.Instance.GetCraftingData(itemType);

            count = new InventoryData();
        }

        //ui 변경하기

        
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
        if (fpress)//&& Installer.IsInstall)
        {
            fPressTime += Time.deltaTime;
            if (fPressTime >= 0.75f)
            {
                fpress = false;
                fPressTime = 0;

                if (Crafter.IsCanCraftirng(EnumCursor.Current))
                {
                    Crafter.Craft(EnumCursor.Current);
                }
                else
                {
                    //경고 띄우기
                }
            }
        }
        else
        {
            fPressTime = 0;
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
        if (enumTypes == null || enumTypes.Length == 0)
        {
            throw new ArgumentException("하나 이상의 enum 타입이 필요합니다.");
        }

        foreach (Type enumType in enumTypes)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException(nameof(enumTypes), "enum 타입이 null입니다.");
            }

            if (!enumType.IsEnum)
            {
                throw new ArgumentException($"{enumType.Name}은 enum 타입이 아닙니다.");
            }

            Array rawValues = Enum.GetValues(enumType);

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

        if (groupIndex < 0 )
        {
            groupIndex = enumValueGroups.Count-1;
        }

        valueIndex = enumValueGroups[groupIndex].Length - 1;
    }

    public void Reset()
    {
        groupIndex = 0;
        valueIndex = 0;
    }
}