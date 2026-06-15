using UnityEngine;
using System.Collections.Generic;

public class TurretTargetFinder : MonoBehaviour
{
    public List<EnemyController> TargetList;

    public bool IsOnlyEnemy;

    public Transform GetTarget()
    {
        if(TargetList.Count == 0)
        {
            return null;
        }
        else
        {
            return TargetList[0].transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            ListEnter(other.transform.parent.GetComponent<EnemyController>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            ListExit(other.transform.parent.GetComponent<EnemyController>());
        }
    }

    public void ListEnter(EnemyController enemy)
    {
        TargetList.Add(enemy);
        enemy.OnDead += ListExit;
    }

    public void ListExit(EnemyController enemy)
    {
        TargetList.Remove(enemy);
        enemy.OnDead -= ListExit;
    }
}
