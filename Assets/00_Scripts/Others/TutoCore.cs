using UnityEngine;

public class TutoCore : InteractableObject
{
    MainCoreController controller;

    public GameObject OnObject;

    private void Awake()
    {
        controller = GetComponent<MainCoreController>();
    }

    public override void Interaction()
    {
        if(TutorialManager.Instance.Current_State == Game_State.Attack)
        {
            OnObject.SetActive(true);
            TutorialManager.Instance.Ready();
            controller.CoreActived();
            TutorialManager.Instance.NextInfo();
        }
        else if(TutorialManager.Instance.Current_State == Game_State.Ready)
        {
            TutorialManager.Instance.StartWave();
            End = true;
            TutorialManager.Instance.NextInfo();
        }
    }

    public override string GetInfo()
    {
        if (TutorialManager.Instance.Current_State == Game_State.Attack)
        {
            return "코어 활성화";
        }
        else if (TutorialManager.Instance.Current_State == Game_State.Ready)
        {
            return "웨이브 시작";
        }
        return "";
    }

    public override bool IsInteract()
    {
        if ((TutorialManager.Instance.Current_State == Game_State.Attack&& TutorialManager.Instance.IsAllKill()) || TutorialManager.Instance.Current_State == Game_State.Ready)
        {
            return !End;
        }
        return false;
    }
}
