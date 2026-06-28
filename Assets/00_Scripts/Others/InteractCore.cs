using UnityEngine;

public class InteractCore : InteractableObject
{
    MainCoreController controller;

    public GameObject OnObject;

    private void Awake()
    {
        controller = GetComponent<MainCoreController>();
    }

    public override void Interaction()
    {
        if(GameManager.Instance.Current_State == Game_State.Attack)
        {
            OnObject.SetActive(true);
            GameManager.Instance.CheckAttackEnd();
            controller.CoreActived();
        }
        else if(GameManager.Instance.Current_State == Game_State.Ready)
        {
            GameManager.Instance.WaveStart();
        }
        else if(GameManager.Instance.Current_State == Game_State.Waving)
        {
            GameManager.Instance.MapEnding();
        }
    }

    public override string GetInfo()
    {
        if (GameManager.Instance.Current_State == Game_State.Attack)
        {
            return "코어 활성화";
        }
        else if (GameManager.Instance.Current_State == Game_State.Ready)
        {
            return "웨이브 시작";
        }
        else if (GameManager.Instance.Current_State == Game_State.Waving)
        {
            return "포기";
        }
        return "";
    }

    public override bool IsInteract()
    {
        if ((GameManager.Instance.Current_State == Game_State.Attack&& GameManager.Instance.isAttackEnd()) || GameManager.Instance.Current_State == Game_State.Ready || GameManager.Instance.Current_State == Game_State.Waving)
        {
            return true;
        }
        return false;
    }
}
