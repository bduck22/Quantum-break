using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpawnManager : MonoBehaviour
{
    public TurretController[] TurretPrefabs;

    private Dictionary<Turret_Type, Queue<TurretController>> pools;

    public Transform[] TurretParents;

    public Transform TurretPoolsParent;


    public void Init()
    {
        TurretParents = new Transform[TurretPrefabs.Length];
        pools = new Dictionary<Turret_Type, Queue<TurretController>>();

        BaseSpawning();
    }

    void BaseSpawning()
    {
        for (int i=0; i < TurretPrefabs.Length; i++)
        {
            TurretParents[i] = new GameObject().transform;
            TurretParents[i].parent = TurretPoolsParent;

            TurretParents[i].name = ((Turret_Type)i).ToString();

            pools[(Turret_Type)i] = new Queue<TurretController>();

            for (int j = 0; j < GameDataManager.Instance.GetCraftingData((Turret_Type)i).MaxCount; j++)
            {
                TurretController turret = Instantiate(TurretPrefabs[i].gameObject, TurretParents[i]).GetComponent<TurretController>();

                turret.gameObject.SetActive(false);

                turret.Data = GameDataManager.Instance.GetTurretData((Turret_Type)i);
                turret.DefaultInit(this);

                InPool((Turret_Type)(i), turret);
            }
        }
    }

    public void InPool(Turret_Type Type, TurretController Controller)
    {
        pools[Type].Enqueue(Controller);
    }

    public void SetTurret(Turret_Type type, Vector3 Position)
    {
        TurretController Turret = pools[type].Dequeue();

        Turret.Init(Position);
    }
}
