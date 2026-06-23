using UnityEngine;

public class InteractCore : InteractableObject
{
    MainCoreController controller;

    private void Awake()
    {
        controller = GetComponent<MainCoreController>();
    }

    public override void Interaction()
    {
        if(GameManager.Instance.Current_State == Game_State.Attack)
        {
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
        throw new System.NotImplementedException();
    }
}
