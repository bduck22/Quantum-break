using System;
using UnityEngine;
using System.Collections.Generic;

public class BuffController : MonoBehaviour
{
    public bool IsEnemy;

    [SerializeField]
    EnemyController Econtroller;
    [SerializeField]
    PlayerController Pcontroller;

    public List<BuffBase> Buffs = new List<BuffBase>();

    private void Awake()
    {
        if(gameObject.layer == 8)
        {
            Econtroller = GetComponent<EnemyController>();
        }
        else
        {
            Pcontroller = GetComponent<PlayerController>();
        }
    }

    private void Update()
    {
        if (Buffs.Count > 0)
        {
            for (int i = 0; i < Buffs.Count; i++)
            {
                BuffBase buff = Buffs[i];

                if (buff.Tick())
                {
                    Buffs.RemoveAt(i--);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 13)
        {
            BuffBase buff = other.GetComponent<BuffBase>();
            if (buff.TargetEnter())
            {
                //혹시 모르잖아
            }
            else
            {
                BuffAdded(other.GetComponent<BuffBase>());
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == 13)
        {
            BuffBase buff = other.GetComponent<BuffBase>();
            if (buff.TargetExit())
            {
                BuffDeleted(buff);
            }
        }
    }

    void BuffDeleted(BuffBase buff)
    {
        buff.BuffDeactived();
    }

    void BuffAdded(BuffBase originbuff)
    {
        bool IsDuplication = false;
        foreach (BuffBase buffs in Buffs)
        {
            if(originbuff.Data.Type == buffs.Data.Type)
            {
                buffs.OriginalBuff = originbuff;
                buffs.Refresh();
                IsDuplication = true;
            }
        }

        if (IsDuplication)
        {
            return;
        }

        if (IsEnemy)
        {
            originbuff.SetTarget(Econtroller);
        }
        else
        {
            originbuff.SetTarget(Pcontroller);
        }
        BuffBase buff = originbuff.Clone();

        buff.OriginalBuff = originbuff;

        buff.Refresh();
        Buffs.Add(buff);
    }
}
