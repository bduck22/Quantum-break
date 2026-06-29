using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TurretSpawnManager : MonoBehaviour
{
    public TurretController[] TurretPrefabs;

    private Dictionary<Turret_Type, Queue<TurretController>> pools;

    public Transform[] TurretParents;

    public Transform TurretPoolsParent;

    public List<TurretController> SpawnTurrets;

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

            Turret_Type type = TurretPrefabs[i].Data.Type;

            TurretParents[i].name = (type).ToString();

            pools[type] = new Queue<TurretController>();

            for (int j = 0; j < GameDataManager.Instance.GetCraftingData(type).MaxCount; j++)
            {
                TurretController turret = Instantiate(TurretPrefabs[i].gameObject, TurretParents[i]).GetComponent<TurretController>();



                turret.gameObject.SetActive(false);

                turret.Data = GameDataManager.Instance.GetTurretData(type);
                turret.DefaultInit(this);

                InPool(type, turret);
            }
        }
    }

    public void InPool(Turret_Type Type, TurretController Controller)
    {
        pools[Type].Enqueue(Controller);
        if (SpawnTurrets.Contains(Controller))
        {
            SpawnTurrets.Remove(Controller);
        }
    }

    public void SetTurret(Turret_Type type, Vector3 Position)
    {
        TurretController Turret = pools[type].Dequeue();

        SpawnTurrets.Add(Turret);

        Turret.Init(Position);
    }

    public void AllBack()
    {
        for (int i = 0; i < SpawnTurrets.Count;)
        {
            TurretController turret = SpawnTurrets[i];
            turret.UnInstall();
        }
    }
}
